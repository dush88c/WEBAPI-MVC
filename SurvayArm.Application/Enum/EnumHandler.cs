
using System.ComponentModel.DataAnnotations;

namespace SurvayArm.Application.Enum
{
    public enum EnumSurvayLanguageType
    {
        Sinhala = 1,
        English ,
        Tamil
    }

    public enum EnumFieldDependantType 
    { 
        [Display(Name = "Input number validate")]
        InputNumberValidate  = 1,
        [Display(Name = "Generate text boxes")]
        GenerateTextbox ,
        [Display(Name = "Terminate the survay")]
        TerminateSurvay,
        [Display(Name = "Hide Question")]
        HideQuestion ,
        [Display(Name = "Options Hide in other")]
        NotIn,
        [Display(Name = "Check options by default in other")]
        In, 

    }

    public enum EnumSurvayTargetSettings 
    {
        [Display(Name = "Budhisum")]
        Budhism =1,
        [Display(Name = "Hinduism")]
        Hinduism ,
        [Display(Name = "Islam")]
        Islam ,
        [Display(Name = "Christian(RC)")]
        ChristianRc,
        [Display(Name = "Christian(Other)")]
        ChristianOther ,

    }

    public enum EnumDeviceBrand
    {
        [Display(Name = "Huawei")]
        Huwavi = 1,
        [Display(Name = "Sony")]
        Sony,
        [Display(Name = "Samsung")]
        Samsung,
        [Display(Name = "HTC")]
        HTC,
        [Display(Name = "LG")]
        LG,
        [Display(Name = "OPPO")]
        OPPO,
        [Display(Name = "Lenovo")]
        Lenovo,
        [Display(Name = "Motorola")]
        Motorola,
        [Display(Name = "Asus")]
        Asus,
        [Display(Name = "Other")]
        Other, 

    }
}
