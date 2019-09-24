namespace SurvayArm.Application.Dto
{
    public class FieldDependantDto 
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

        public FieldDto Field { get; set; }
        public FieldDto Field1 { get; set; } 
    }
}
