using AutoMapper;
using SurvayArm.API.Models.AppModel;
using SurvayArm.Application.Dto;

namespace SurvayArm.API.AutoMapper
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            CreateMap<SurvayApiModel, SurvayDto>().PreserveReferences();
            CreateMap<SurvayDto, SurvayApiModel>().PreserveReferences();
            CreateMap<FieldApiModel, FieldDto>().PreserveReferences()
                .ForMember(o => o.FieldOption, dto => dto.MapFrom(s => s.field_options));
            CreateMap<FieldDto, FieldApiModel>().PreserveReferences()
                .ForMember(o => o.field_options, dto => dto.MapFrom(s => s.FieldOption));
            CreateMap<FieldOptionsApiModel, FieldOptionDto>().PreserveReferences()
                    .ForMember(o => o.Options, dto => dto.MapFrom(s => s.options));
            CreateMap<FieldOptionDto, FieldOptionsApiModel>().PreserveReferences()
                .ForMember(o => o.options, dto => dto.MapFrom(s => s.Options));
            CreateMap<OptionApiModel, OptionDto>().PreserveReferences()
                .ForMember(o => o.Checked, dto => dto.MapFrom(s => s.@checked))
                .ForMember(o => o.Label, dto => dto.MapFrom(s => s.label));
            CreateMap<OptionDto, OptionApiModel>().PreserveReferences()
                .ForMember(o => o.@checked, dto => dto.MapFrom(s => s.Checked))
                .ForMember(o => o.label, dto => dto.MapFrom(s => s.Label));

            CreateMap<FieldDependantApiModel, FieldDependantDto>().PreserveReferences();
            CreateMap<FieldDependantDto, FieldDependantApiModel>().PreserveReferences();

        }
    }
}