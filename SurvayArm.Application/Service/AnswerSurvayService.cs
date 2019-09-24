using SurvayArm.Application.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using SurvayArm.Application.Dto;
using SurvayArm.Data.UOW;
using AutoMapper;
using log4net;
using SurvayArm.Data.Model;
using System.Data;
using System.IO;
using System.Configuration;

namespace SurvayArm.Application.Service
{
    public class AnswerSurvayService : IAnswerSurvayService, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;
        private readonly ISurvayTypeService _survayTypeService;
        private List<string> Includes => new List<string>() { "AnswerFields", "AnswerFields.MatrixAnswers", "Client", "SurvayType" ,"Survay" , "SurvayType.Fields" , "DeviceManager",
                        "SurvayType.Fields.FieldOption" ,"SurvayType.Fields.FieldOption.Options" , "SurvayType.Fields.FieldOption.MatrixHeaders" , "SurvayType.Fields.FieldOption.MatrixRows"};

        public AnswerSurvayService(IUnitOfWork unitOfWork, IMapper mapper, ISurvayTypeService survayService)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(AnswerSurvayService));
            _mapper = mapper;
            _survayTypeService = survayService;

        }

        public List<AnswerSurvayDto> GetAll()
        {
            try
            {
                var entities = _unitOfWork.GetRepository<AnswerSurvay>().GetAll();
                var dtos = _mapper.Map<List<AnswerSurvay>, List<AnswerSurvayDto>>(entities.ToList());
                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void Insert(AnswerSurvayDto dto)
        {
            try
            {
                var metrixQ = dto.AnswerFields.Where(x => x.Type.Contains("mulitiSelect-matrix"));
                if (metrixQ != null && metrixQ.Any()) 
                {
                    foreach (var mQ in metrixQ)
                    {
                        // since mobile end sending matrix answer in dulicated way , need to removed them and concatinating them
                        var newMatrixList = new List<MatrixAnswerDto>();
                        foreach (var answer in mQ.MatrixAnswers.GroupBy(x=>x.Row))
                        {
                            var selectedHeaders = string.Join(",", answer.Select(x => $"{x.HeaderList}"));
                            var newMatrixANswer = new MatrixAnswerDto()
                            {
                                Row = answer.Key,
                                HeaderList = selectedHeaders
                            };
                            newMatrixList.Add(newMatrixANswer);
                        }

                        mQ.MatrixAnswers = newMatrixList;
                    }
                }

                var entity = _mapper.Map<AnswerSurvayDto, AnswerSurvay>(dto);
                _unitOfWork.GetRepository<AnswerSurvay>().Insert(entity);
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        public DataSet ExportToCsv(int survayId)
        {
            try
            {
                var allTypeanswers = GetAnswersBySurvayId(survayId).GroupBy(x=>x.SurvayTypeID);
                var dataExtractionWithSurvayType = new Dictionary<SurvayTypeDto, IEnumerable<AnswerSurvayDto>>();
                var answerDataSet = new DataSet("survayAnswerDataSet");

                foreach (var answerType in allTypeanswers)
                {
                        var survyTypeId = answerType.Key;
                        var answerDataTable = InsertAnswersIntoDataTable(survyTypeId);
                        answerDataSet.Tables.Add(answerDataTable); 
                }

                return answerDataSet;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }
        
        private DataTable InsertAnswersIntoDataTable(int survayTypeId)
        {
            var answers = GetSurvayAnswerBySurvayType(survayTypeId);
            if (answers ==null)
            {
                return null;
            }
           
            var survayType = answers.First().SurvayType;
            var allQuestions = answers.First().SurvayType?.Fields.OrderBy(o => o.OrderNo).ToList();
            var answerDataTable = SetDataTableColumns(survayType);
            
            var index = 0;
            var answerRow = new List<object>();
            foreach (var answer in answers)
            {
                index++;
                answerRow.Add(index);
                answerRow.Add(answer.StartTime?.ToString("g")); // start time
                answerRow.Add(answer.EndTime?.ToString("g"));  // end time
                answerRow.Add(DateTime.Now);
                answerRow.Add(answer.DeviceManager.DeviceId);

                GetRowOfAnswerValue(answer, allQuestions , answerRow);
                answerDataTable.Rows.Add(answerRow.ToArray());
                answerRow.Clear();
            }

            return answerDataTable;
        }

        private DataTable SetDataTableColumns(SurvayTypeDto survayType)
        {
            var languageName = GetLanguageName(survayType.LanguageType);
            var xlSheetName = $"Answers{survayType.Survay?.Id}{languageName}";

            var answerDataTable = new DataTable(xlSheetName);
            var questions = survayType.Fields.OrderBy(o => o.OrderNo);

            answerDataTable.Columns.Add("IndexNo");
            answerDataTable.Columns.Add("Start");
            answerDataTable.Columns.Add("End");
            answerDataTable.Columns.Add("Today");
            answerDataTable.Columns.Add("DeviceId");
            
            foreach (var question in questions)
            {
                if (question.Field_Type.ToLower() == "radio")
                {
                    answerDataTable.Columns.Add(question.Label);                    
                    continue;

                }
                else if (question.Field_Type.ToLower() == "checkboxes")
                {
                    if (question.FieldOption.Include_other_option)
                    {
                        answerDataTable.Columns.Add($"{question.Label}/Other");                       
                    }

                    foreach (var checkboxQ in question.FieldOption.Options.OrderBy(x => x.Id))
                    {
                        answerDataTable.Columns.Add($"{question.Label}/{checkboxQ.Label}");                        
                    }
                    continue;
                }
                else if (question.Field_Type.ToLower() == "checkwithtext")
                {
                    if (question.FieldOption.Include_other_option)
                    {
                        answerDataTable.Columns.Add($"{question.Label}/Other");                        
                    }
                    foreach (var checkboxQ in question.FieldOption.Options.OrderBy(x => x.Id))
                    {
                        answerDataTable.Columns.Add($"{question.Label}/{checkboxQ.Label}");                        
                    }
                    continue;

                }
                else if (question.Field_Type.ToLower() == "mulitiselect-matrix" || question.Field_Type.ToLower() == "single-matrix")
                {
                    foreach (var row in question.FieldOption.MatrixRows.OrderBy(x => x.Id))
                    {
                        foreach (var header in question.FieldOption.MatrixHeaders.OrderBy(c => c.Id))
                        {
                            answerDataTable.Columns.Add($"{question.Label}_{row.Label}_{header.Label}");                            
                        }

                    }
                    continue;

                }                
                else
                {
                    answerDataTable.Columns.Add(question.Label);
                    
                }

            }

            answerDataTable.Columns.Add("Res_Name");
            answerDataTable.Columns.Add("Res_Address");
            answerDataTable.Columns.Add("Res_Tele");
            answerDataTable.Columns.Add("Res_Age");

            return answerDataTable;
        }

        private void GetRowOfAnswerValue(AnswerSurvayDto answer, List<FieldDto> allQuestions , List<object> answerRow)
        {
            foreach (var field in allQuestions) 
            {
                try
                {
                    var fieldAnswer = answer.AnswerFields.FirstOrDefault(x => x.FieldId == field.Id);

                    if (fieldAnswer != null)
                    {
                        if (fieldAnswer.Type.ToLower() == "radio" )
                        {
                             GetRadioQuestionAnswerValue(allQuestions , field.Id, fieldAnswer, answerRow);
                             continue;                            
                        }
                        else if (fieldAnswer.Type.ToLower() == "checkboxes")
                        {
                            GetCheckboxQuestionAnswerValue(allQuestions, field.Id, fieldAnswer, answerRow);
                            continue;
                        }
                        else if(fieldAnswer.Type.ToLower() == "checkwithtext")
                        {
                            GetCheckWithTextQuestionAnswerValue(allQuestions, field.Id, fieldAnswer, answerRow);
                            continue;
                        }
                        else if (fieldAnswer.Type.ToLower() == "number" || fieldAnswer.Type.ToLower() == "text" || fieldAnswer.Type.ToLower() == "paragraph")
                        {
                            answerRow.Add(fieldAnswer.Answer);
                            continue;
                        }                        
                        else if (fieldAnswer.Type.ToLower() == "mulitiselect-matrix" || fieldAnswer.Type.ToLower() == "single-matrix")
                        {
                             GetMatrixQuestionAnswerValue(allQuestions, field.Id, fieldAnswer, answerRow);
                             continue;
                        }
                        else if (fieldAnswer.Type.ToLower() == "ranking-number")
                        {
                            answerRow.Add(fieldAnswer.Answer);
                            continue;
                        }
                        else if (fieldAnswer.Type.ToLower() == "ranking-text")
                        {
                            answerRow.Add(fieldAnswer.Answer);
                            continue;
                        }
                        else if (fieldAnswer.Type.ToLower() == "fileupload")
                        {
                            var filePathName = GetUploadFolderLocation(answer, fieldAnswer);
                            answerRow.Add(filePathName);
                            continue;
                        }

                        answerRow.Add(string.Empty);
                        _log.Info($@"Info : Answer Question Id : {fieldAnswer.Id} , type : ""{fieldAnswer.Type}""  is not match with application allowed types , in survay Type Id : {answer.SurvayTypeID}, It might be type mismatching.");
                    }
                    else
                    {
                        var columnsCount = GetColumnsCountCorrespondingToSurvayQuestion(field);
                        AddingEmptyStringToColumns(answerRow, columnsCount);
                        _log.Info($"Info : Question Id : {field} is not found in answer list , survay Id : {answer.Id}, It might be an optional question.");
                    }
                    
                }
                catch (Exception e)
                {
                    _log.Error($"Error :  {e}");
                    answerRow.Add(string.Empty);                    
                }
            }

            answerRow.Add($"{answer.Client?.FirstName ?? string.Empty} {answer.Client?.LastName ?? string.Empty}");
            answerRow.Add(answer.Client?.Address ?? string.Empty);
            answerRow.Add(answer.Client?.MobileNumber ?? string.Empty);
            answerRow.Add(DateTime.Now.Year - answer.Client?.BirthOfDate?.Year);
            
        }

        private string GetUploadFolderLocation(AnswerSurvayDto answer, AnswerFieldDto answerField) 
        {
            try
            {
                var path = ConfigurationManager.AppSettings["AnswerDocumentDirectory"];
                var ftpPath = ConfigurationManager.AppSettings["FtpPath"];

                string directoryName = Path.Combine(path, "FilesOfAnswer");
                string survayFolderName = Path.Combine(directoryName, answer?.SurvayId.ToString());
                string survayTypeFolderName = Path.Combine(survayFolderName, answer?.SurvayTypeID.ToString());
                string clientFolder  = Path.Combine(survayTypeFolderName, $"{answer?.ClientId.ToString()}");

                if (Directory.Exists(clientFolder))
                {
                    return $"{ftpPath}/{answer?.SurvayId.ToString()}/{answer?.SurvayTypeID.ToString()}/{answer?.ClientId.ToString()}/{answerField.Answer}";
                }

                return "File not found";
            }
            catch (Exception ex)
            {
                _log.Error($"Error :  {ex}");
                throw;
            }
        }

        private string GetLanguageName (int languageId)
        {
            if (languageId == 1)
            {
                return "Sinhala";
            }
            else if(languageId == 2)
            {
                return "English";
            }
            return "Tamil";
        }
        private IEnumerable<AnswerSurvayDto> GetSurvayAnswerBySurvayType(int survayTypeId)
        {
            try
            {
                var entyties = _unitOfWork.GetRepository<AnswerSurvay>().GetManyWithInclude(x=>x.SurvayTypeID == survayTypeId, Includes.ToArray()).ToList();
                return _mapper.Map<IEnumerable<AnswerSurvay>, IEnumerable<AnswerSurvayDto>>(entyties);
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        private List<AnswerSurvayDto> GetAnswersBySurvayId(int survayId)
        {
            try
            {
                var entities = _unitOfWork.GetRepository<AnswerSurvay>().GetMany(c => c.SurvayId == survayId);
                var data = entities?.ToList();
                return _mapper.Map<List<AnswerSurvay>, List<AnswerSurvayDto>>(data);

            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }
        
        public int GetSurvayCountHasDone(int survayId)
        {
            try
            {
               return _unitOfWork.GetRepository<AnswerSurvay>().GetMany(c => c.SurvayId == survayId).Count();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        private void Save()
        {
            _unitOfWork.Save();
        }

        #region Get Question's answer value by checking the type of it

        private void GetRadioQuestionAnswerValue(List<FieldDto> allQuestions, int questionId, AnswerFieldDto questionAnswer, List<object> answerRow) 
        {
            try
            {
                var question = allQuestions.FirstOrDefault(x => x.Id == questionId && string.Equals(x.Field_Type, "radio", StringComparison.CurrentCultureIgnoreCase));

                if (question != null)
                {
                    if (question.FieldOption.Options.Any())
                    {
                        var radioAnswer = new List<string>();
                        var questionOptions = question.FieldOption.Options.OrderBy(x => x.Id).ToList();
                        var OtherOptionIndex = questionOptions.Count + 1;

                        // Check if user has selected the "Includeother" option
                        if (!string.IsNullOrEmpty(questionAnswer.IncludeOther))
                        {
                            answerRow.Add(questionAnswer.IncludeOther);
                            return;
                        }

                        var selectedOptions = questionAnswer.Answer?.Split(',').ToArray();

                        for (int j = 0; j < questionOptions.Count; j++)
                        {
                            if (selectedOptions.Any(x => Convert.ToInt32(x) == questionOptions[j].Id))
                            {
                                answerRow.Add(questionOptions[j].Label);
                                break;
                            }                            
                        }
                    }
                }               
               
            }
            catch (Exception e)
            {
                _log.Error(e);
               
            }

        }

        private void GetCheckboxQuestionAnswerValue(List<FieldDto> allQuestions, int questionId, AnswerFieldDto questionAnswer, List<object> answerRow)
        {
            try
            {
                var question = allQuestions.FirstOrDefault(x => x.Id == questionId && string.Equals(x.Field_Type, "checkboxes", StringComparison.CurrentCultureIgnoreCase));

                if (question != null)
                {
                    if (question.FieldOption.Options.Any())
                    {
                        var checkboxAnswer = new List<string>();
                        var questionOptions = question.FieldOption.Options.OrderBy(x => x.Id).ToList();

                        if (question.FieldOption.Include_other_option)
                        {
                            if (!string.IsNullOrEmpty(questionAnswer.IncludeOther))
                            {
                                answerRow.Add(questionAnswer.IncludeOther);
                            }
                            else
                            {
                                answerRow.Add(0);
                            }
                        }

                        var selectedOptions = questionAnswer.Answer?.Split(',').ToArray();

                        foreach (var checkboxQ in question.FieldOption.Options.OrderBy(x => x.Id))
                        {
                            if (selectedOptions.Any(c=> Convert.ToInt32(c) == checkboxQ.Id))
                            {
                                answerRow.Add(1);
                            }
                            else
                            {
                                answerRow.Add(0);
                            }
                        }
                    }
                }
                                
            }
            catch (Exception e)
            {
                _log.Error(e);
               
            }

        }

        private void GetCheckWithTextQuestionAnswerValue(List<FieldDto> allQuestions, int questionId, AnswerFieldDto questionAnswer , List<object> answerRow)
        {
            try
            {
                var checkWithText = questionAnswer.Answer?.Split('|');
                if (checkWithText == null && !checkWithText.Any())
                {
                    _log.Info($@"Info : Answer question Id : {questionAnswer.Id} and type : ""checkWithText"" ,answer value passed is not in valid format , This should be like ""12112,value|8181,value"" ");
                    return;
                }

                var question = allQuestions.FirstOrDefault(x => x.Id == questionId && string.Equals(x.Field_Type, "checkWithText", StringComparison.CurrentCultureIgnoreCase));

                if (question != null)
                {
                    if (question.FieldOption.Options.Any())
                    {
                        var checkboxAnswer = new List<string>();
                        var questionOptions = question.FieldOption.Options.OrderBy(x => x.Id).ToList();

                        if (question.FieldOption.Include_other_option)
                        {
                            if (!string.IsNullOrEmpty(questionAnswer.IncludeOther))
                            {
                                answerRow.Add(questionAnswer.IncludeOther);
                            }
                        }

                        var selectedOptions = questionAnswer.Answer?.Split('|').Select(x => x.Split(','))
                                        .ToDictionary(c => Convert.ToInt32(c[0].Trim()), c => c[1]);

                        foreach (var checkBoxWithTextQ in questionOptions)
                        {
                            if (selectedOptions.TryGetValue(checkBoxWithTextQ.Id , out string value))
                            {
                                answerRow.Add(value);
                            }
                            else
                            {
                                answerRow.Add(string.Empty);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
            }

        }

        private void GetMatrixQuestionAnswerValue(List<FieldDto> allQuestions, int field, AnswerFieldDto fieldAnswer, List<object> answerRow)
        {
            try
            {
                if (fieldAnswer.MatrixAnswers == null || !fieldAnswer.MatrixAnswers.Any())
                {
                    _log.Info($"Info :Matrix question : {field} answers are not there");
                    return;
                }

                var matrixAnswers = fieldAnswer.MatrixAnswers;
                var matrixQuestionsHeader = allQuestions.FirstOrDefault(x => x.Id == field).FieldOption?.MatrixHeaders.OrderBy(x=>x.Id).ToList();
                var matrixQuestionsRow = allQuestions.FirstOrDefault(x => x.Id == field).FieldOption?.MatrixRows.OrderBy(x => x.Id).ToList();

                foreach (var row in matrixQuestionsRow)
                {
                    // check if answer row is equal with question row 
                    var matrixAnswer = matrixAnswers.FirstOrDefault(r => Convert.ToInt32(r.Row.Replace("ROW_" ,"")) == row.Id);
                    
                    if (matrixAnswer == null || string.IsNullOrEmpty(matrixAnswer.HeaderList))
                    {
                        _log.Info($"Info :Matrix question Id : {field} and matrix answer row Id : {row.RowId}  answers are not there");

                        AddingEmptyStringToColumns(answerRow, matrixQuestionsHeader.Count);
                        continue;
                    }

                    var answerHeaders = matrixAnswer.HeaderList.Split(',').Select(i => Convert.ToInt32(i.Replace("HEADER_" ,""))).ToList();

                    if (answerHeaders ==null || !answerHeaders.Any())
                    {
                        _log.Info($"Info :Matrix question Id : {field} and row Id : {row}  and matrixAnswerId : {matrixAnswer.Id} headerList is not valid or no values ");

                        AddingEmptyStringToColumns(answerRow, matrixQuestionsHeader.Count);
                        continue;
                    }

                    foreach (var header in matrixQuestionsHeader)
                    {
                        // check if answer header id equal with question header id
                        if(answerHeaders.Any(c=> c == header.Id))
                        {
                            answerRow.Add(1);
                        }
                        else
                        {
                            answerRow.Add(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e);               
            }
        }


        private void AddingEmptyStringToColumns(List<object> answerRow , int columnCount)
        {
            try
            {
                for (int i = 0; i < columnCount; i++)
                {
                    answerRow.Add(string.Empty);
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        private int GetColumnsCountCorrespondingToSurvayQuestion(FieldDto question)
        {
            try
            {
                var columnsCount = 0;
                if (question.Field_Type.ToLower() == "checkboxes" || question.Field_Type.ToLower() == "checkwithtext")
                {
                    if (question.FieldOption.Include_other_option)
                    {
                        columnsCount++;
                    }

                    columnsCount += question.FieldOption.Options.Count();
                }
                else if (question.Field_Type.ToLower() == "mulitiselect-matrix" || question.Field_Type.ToLower() == "single-matrix")
                {
                    columnsCount = question.FieldOption.MatrixRows.Count() * question.FieldOption.MatrixHeaders.Count();
                }
                else
                {
                    columnsCount++;
                }

                return columnsCount;
            }
            catch (Exception e)
            {
                _log.Error(e);
                return 0;
            }
        }

        #endregion

        #region Implementing IDiosposable...

        #region private dispose variable declaration...

        private bool _disposed;

        #endregion

        /// <summary>
        ///     Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
            }
            _disposed = true;
        }

        /// <summary>
        ///     Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
