//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SurvayArm.Data.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Field
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Field()
        {
            this.AnswerFields = new HashSet<AnswerField>();
            this.FieldDependants = new HashSet<FieldDependant>();
            this.FieldDependants1 = new HashSet<FieldDependant>();
        }
    
        public int Id { get; set; }
        public int SurvayId { get; set; }
        public string Label { get; set; }
        public string Field_Type { get; set; }
        public bool Required { get; set; }
        public string VideoUrl { get; set; }
        public bool IsImageUpload { get; set; }
        public bool IsVoiceUpload { get; set; }
        public int OrderNo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnswerField> AnswerFields { get; set; }
        public virtual SurvayType SurvayType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FieldDependant> FieldDependants { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FieldDependant> FieldDependants1 { get; set; }
        public virtual FieldOption FieldOption { get; set; }
    }
}
