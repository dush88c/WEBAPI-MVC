using App.Models.AppModel;
using AutoMapper;
using SurvayArm.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Automapper
{
    public class WebMappingProfile : Profile 
    {
        public WebMappingProfile()
        {
            CreateMap<SurvayViewModel, SurvayDto>().PreserveReferences();
            CreateMap<SurvayDto, SurvayViewModel>().PreserveReferences();
            CreateMap<SurvayTypeViewModel, SurvayTypeDto>().PreserveReferences();
            CreateMap<SurvayTypeDto, SurvayTypeViewModel>().PreserveReferences();
            CreateMap<FieldViewModel, FieldDto>().PreserveReferences()
                .ForMember(o => o.FieldOption, dto => dto.MapFrom(s => s.field_options));                
            CreateMap<FieldDto, FieldViewModel>().PreserveReferences()
                .ForMember(o => o.field_options, dto => dto.MapFrom(s => s.FieldOption));
            CreateMap<FieldOptionsViewModel, FieldOptionDto>().PreserveReferences()
                    .ForMember(o=>o.Options , dto=>dto.MapFrom(s=>s.options))
                    .ForMember(o => o.MatrixRows, dto => dto.MapFrom(s => s.matrixRowOptions))
                    .ForMember(o => o.MatrixHeaders, dto => dto.MapFrom(s => s.matrixHeaderOptions));
            CreateMap<FieldOptionDto, FieldOptionsViewModel>().PreserveReferences()
                .ForMember(o => o.options, dto => dto.MapFrom(s => s.Options))
                .ForMember(o => o.matrixRowOptions, dto => dto.MapFrom(s => s.MatrixRows))
                .ForMember(o => o.matrixHeaderOptions, dto => dto.MapFrom(s => s.MatrixHeaders));
            CreateMap<OptionViewModel, OptionDto>().PreserveReferences()
                .ForMember(o => o.Checked, dto => dto.MapFrom(s => s.@checked))
                .ForMember(o => o.Label, dto => dto.MapFrom(s => s.label));
            CreateMap<OptionDto, OptionViewModel>().PreserveReferences()
                .ForMember(o => o.@checked, dto => dto.MapFrom(s => s.Checked))
                .ForMember(o => o.label, dto => dto.MapFrom(s => s.Label));

            CreateMap<MatrixHeaderDto, MatrixHeaderViewModel>().PreserveReferences()
                .ForMember(o => o.label, dto => dto.MapFrom(s => s.Label));
            CreateMap<MatrixHeaderViewModel, MatrixHeaderDto>().PreserveReferences()
                .ForMember(o => o.Label, dto => dto.MapFrom(s => s.label));

            CreateMap<MatrixRowDto, MatrixRowViewModel>().PreserveReferences()
                .ForMember(o => o.label, dto => dto.MapFrom(s => s.Label));
            CreateMap<MatrixRowViewModel, MatrixRowDto>().PreserveReferences()
                .ForMember(o => o.Label, dto => dto.MapFrom(s => s.label));


            CreateMap<FieldDependantViewModel, FieldDependantDto>().PreserveReferences();
            CreateMap<FieldDependantDto, FieldDependantViewModel>().PreserveReferences();

            CreateMap<SurvaySettingViewModel, SurvaySettingDto>().PreserveReferences();
            CreateMap<SurvaySettingDto, SurvaySettingViewModel>().PreserveReferences();

            CreateMap<SurvayTargetViewModel, SurvayTargetDto>().PreserveReferences();
            CreateMap<SurvayTargetDto, SurvayTargetViewModel>().PreserveReferences();

            CreateMap<DeviceManagerViewModel , DeviceManagerDto>().PreserveReferences();
            CreateMap<DeviceManagerDto, DeviceManagerViewModel>().PreserveReferences();

        }
    }
}