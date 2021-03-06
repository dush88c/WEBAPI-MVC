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
    
    public partial class FieldOption
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FieldOption()
        {
            this.Options = new HashSet<Option>();
            this.MatrixHeaders = new HashSet<MatrixHeader>();
            this.MatrixRows = new HashSet<MatrixRow>();
        }
    
        public int FieldId { get; set; }
        public string Size { get; set; }
        public string Min_max_length_units { get; set; }
        public int Minlength { get; set; }
        public bool Include_other_option { get; set; }
        public bool Integer_only { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Option> Options { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MatrixHeader> MatrixHeaders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MatrixRow> MatrixRows { get; set; }
        public virtual Field Field { get; set; }
    }
}
