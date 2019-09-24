using System;
using System.Collections.Generic;

namespace SurvayArm.API.Models.AppModel
{
    public class SurvayApiModel 
    {
        public SurvayApiModel()
        {
            SurvayTypes = new List<SurvayTypeApiModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int NoOfQuestion { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public List<SurvayTypeApiModel> SurvayTypes { get; set; }
    }

    public class SurvayTypeApiModel 
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
        public SurvayTypeApiModel Survay { get; set; }

        public List<FieldApiModel> fields { get; set; } 
    }

    public class FieldApiModel
    {
        public int id { get; set; }
        public string label { get; set; }
        public string field_type { get; set; }
        public bool required { get; set; }
        //[JsonIgnore]
        public FieldOptionsApiModel field_options { get; set; } 
        //public string cid { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }

    public class FieldOptionsApiModel 
    {
        public int fieldId { get; set; }
        public string size { get; set; }
        public string min_max_length_units { get; set; }
        public int minlength { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public bool include_other_option { get; set; }
        public bool integer_only { get; set; }

        public List<OptionApiModel> options { get; set; }
    }
     
    public class OptionApiModel
    {
        public int id { get; set; }
        public string label { get; set; }
        public bool @checked { get; set; }
    }

    public class FieldDependantApiModel
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public int DependantFieldId { get; set; }
        public int DependantType { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        //[JsonIgnore]
        public FieldApiModel Field { get; set; }
    }
}