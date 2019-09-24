using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
    public class FieldOptionDto
    {

        public FieldOptionDto()
        {
            this.Options = new List<OptionDto>();
            this.MatrixHeaders = new List<MatrixHeaderDto>();
            this.MatrixRows = new List<MatrixRowDto>();
        }

        public int FieldId { get; set; }
        public string Size { get; set; }
        public string Min_max_length_units { get; set; }
        public int Minlength { get; set; }
        public bool Include_other_option { get; set; }
        public bool Integer_only { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int MatrixRowCount
        {
            get
            {
                return MatrixRows?.Count ?? 0;
            }
        }

        public FieldDto Field { get; set; }
        public List<OptionDto> Options { get; set; }
        public List<MatrixHeaderDto> MatrixHeaders { get; set; }        
        public List<MatrixRowDto> MatrixRows { get; set; }
    }
}
