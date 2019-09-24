using AutoMapper;
using log4net;
using Newtonsoft.Json;
using SurvayArm.API.Helper;
using SurvayArm.API.Models.AppModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace SurvayArm.API.Controllers
{
    [RoutePrefix("api/MediaUpload")]
    public class DocumentUploadController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public DocumentUploadController(IMapper mapper)
        {
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(DocumentUploadController));
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IHttpActionResult> MediaUpload()
        {
            try
            { 
                // Check if the request contains multipart/form-data.  
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

                NameValueCollection formData = provider.FormData;
                //access files  
                IList<HttpContent> files = provider.Files;

               return await SaveFilesInDirectory(files, formData);     
            
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError(e);
            }           

        }

        private async Task<IHttpActionResult> SaveFilesInDirectory(IList<HttpContent> files, NameValueCollection formData)
        {
            try
            {
                if (formData["ClientDetail"] != null)
                {
                    var docRelatedInfo = JsonConvert.DeserializeObject<UploadedClient>(formData["ClientDetail"].ToString());

                    foreach (var file in files)
                    {
                        var thisFileName = file.Headers.ContentDisposition.FileName.Trim('\"');

                        Stream input = await file.ReadAsStreamAsync();
                        var path = WebConfigurationManager.AppSettings["AnswerDocumentDirectory"];

                        if (string.IsNullOrEmpty(path))
                        {
                            path = HttpRuntime.AppDomainAppPath;
                        }

                        string directoryName = Path.Combine(path, "FilesOfAnswer");

                        // check if Directory exist
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        string survayFolderName = Path.Combine(directoryName, docRelatedInfo.SurvayId.ToString());

                        // check if survay directory exist
                        if (!Directory.Exists(survayFolderName))
                        {
                            Directory.CreateDirectory(survayFolderName);
                        }

                        string survayTypeFolderName  = Path.Combine(survayFolderName, docRelatedInfo.SurvayTypeId.ToString());

                        // check if survayType folder exist
                        if (!Directory.Exists(survayTypeFolderName))
                        {
                            Directory.CreateDirectory(survayTypeFolderName);
                        }
                        string clientFolderName = Path.Combine(survayTypeFolderName, docRelatedInfo.ClientName); 

                        // check if client directory in survayType folder exist
                        if (!Directory.Exists(clientFolderName))
                        {
                            Directory.CreateDirectory(clientFolderName);
                        }

                        string filename = Path.Combine(clientFolderName, thisFileName);

                        //Deletion exists file  
                        if (File.Exists(filename))
                        {
                            File.Delete(filename);
                        }

                        using (Stream doc = File.OpenWrite(filename))
                        {
                            input.CopyTo(doc);
                            //close file  
                            doc.Close();
                        }
                    }
                    return Ok();
                }
                return BadRequest($"Infomation related to save documents is not correct or in valid format");
            }
            catch (Exception e)
            {
                _log.Error($"Error : {e}");
                return InternalServerError(e);
            }
        }
    }
}
