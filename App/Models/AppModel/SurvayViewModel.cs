using System;
using System.Collections.Generic;

namespace App.Models.AppModel
{
    public class SurvayViewModel
    {
        public SurvayViewModel()
        {
            SurvayTypes = new List<SurvayTypeViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int NoOfQuestion { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public List<SurvayTypeViewModel> SurvayTypes { get; set; }
    }

    public class SurvayTypeViewModel
    {
        public int Id { get; set; }
        public int SurvayId { get; set; }
        public string Description { get; set; }
        public int LanguageType { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        //[JsonIgnore]
        public SurvayViewModel Survay { get; set; }
        
        public List<FieldViewModel> fields { get; set; }
    }

    public class FieldViewModel
    {
        public int id { get; set; }
        public string label { get; set; }
        public string field_type { get; set; }
        public bool required { get; set; }
        public string videoUrl { get; set; }
        public bool isImageUpload { get; set; } 
        public bool isVoiceUpload { get; set; }
        public int orderNo { get; set; } 

        public FieldOptionsViewModel field_options { get; set; }
        //public string cid { get; set; }
        public System.DateTime CreatedDate { get; set; }

        public List<FieldDependantViewModel> FieldDependants { get; set; }
    }

    public class FieldOptionsViewModel
    {
        public  int fieldId { get; set; } 
        public string size { get; set; }
        public string min_max_length_units { get; set; }         
        public int minlength { get; set; }
        public int min { get; set; }
        public int max { get; set; } 
        public bool include_other_option { get; set; }
        public bool integer_only { get; set; }
       
        public List<OptionViewModel> options { get; set; }
        public List<MatrixHeaderViewModel> matrixHeaderOptions { get; set; }
        public List<MatrixRowViewModel> matrixRowOptions { get; set; } 
    }

    public class OptionViewModel
    { 
        public int id { get; set; } 
        public string label { get; set; }
        public bool @checked { get; set; } 
    }
    public class MatrixHeaderViewModel 
    {
        public int id { get; set; }
        public string label { get; set; }       
    }
    public class MatrixRowViewModel 
    {
        public int id { get; set; }
        public string label { get; set; }        
    }

    public class FieldDependantViewModel 
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public int? DependantFieldId { get; set; }
        public int DependantType { get; set; }
        public string Value { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public FieldViewModel Field { get; set; }
    }   

    public class SurvaySettingViewModel
    {
        public SurvaySettingViewModel()
        {
            SurvayTargets = new List<SurvayTargetViewModel>();
        }

        public int SurvayId { get; set; }
        public int Target { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public SurvayViewModel Survay { get; set; }

        public List<SurvayTargetViewModel> SurvayTargets { get; set; }
    }

    public class SurvayTargetViewModel 
    {
        public int Id { get; set; }
        public int SurvaySettingId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int Target { get; set; }
        public int OptionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        //public SurvaySettingViewModel SurvaySetting { get; set; }
    }
}