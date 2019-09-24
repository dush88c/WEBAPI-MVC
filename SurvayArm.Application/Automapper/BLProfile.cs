using AutoMapper;
using SurvayArm.Application.Dto;
using SurvayArm.Data.Model;

namespace SurvayArm.Application.Automapper
{
    public class BlProfile : Profile 
    {
        public BlProfile() 
        {
            CreateMap<Survay, SurvayDto>().PreserveReferences();
            CreateMap<SurvayDto, Survay>().PreserveReferences();

            CreateMap<SurvaySetting, SurvaySettingDto>().PreserveReferences();
            CreateMap<SurvaySettingDto, SurvaySetting>().PreserveReferences();

            CreateMap<SurvayType, SurvayTypeDto>().PreserveReferences();
            CreateMap<SurvayTypeDto, SurvayType>().PreserveReferences();

            CreateMap<SurvaySupervisor, SurvaySupervisorDto>().PreserveReferences();
            CreateMap<SurvaySupervisorDto, SurvaySupervisor>().PreserveReferences();

            CreateMap<Field, FieldDto>().PreserveReferences();
            CreateMap<FieldDto, Field>().PreserveReferences();

            CreateMap<FieldOption, FieldOptionDto>().PreserveReferences();
            CreateMap<FieldOptionDto, FieldOption>().PreserveReferences();

            CreateMap<Option, OptionDto>().PreserveReferences();
            CreateMap<OptionDto, Option>().PreserveReferences();

            CreateMap<FieldDependantDto, FieldDependant>().PreserveReferences();
            CreateMap<FieldDependant, FieldDependantDto>().PreserveReferences();

            CreateMap<SurvaySettingDto, SurvaySetting>().PreserveReferences();
            CreateMap<SurvaySettingDto, SurvaySetting>().PreserveReferences();

            CreateMap<SurvayTargetDto, SurvayTarget>().PreserveReferences();
            CreateMap<SurvayTarget, SurvayTargetDto>().PreserveReferences();

            CreateMap<SurvaySupervisorDto, SurvaySupervisor>().PreserveReferences();
            CreateMap<SurvaySupervisor, SurvaySupervisorDto>().PreserveReferences();

            CreateMap<UserDto, AspNetUser>().PreserveReferences();
            CreateMap<AspNetUser, UserDto>().PreserveReferences();

            CreateMap<DistrictDto, District>().PreserveReferences();
            CreateMap<District, DistrictDto>().PreserveReferences();

            CreateMap<ProvinceDto, Province>().PreserveReferences();
            CreateMap<Province, ProvinceDto>().PreserveReferences();

            CreateMap<SurvaySupervisorDto, SurvaySupervisor>().PreserveReferences();
            CreateMap<SurvaySupervisor, SurvaySupervisorDto>().PreserveReferences();

            CreateMap<DeviceManager, DeviceManagerDto>().PreserveReferences();
            CreateMap<DeviceManagerDto, DeviceManager>().PreserveReferences();

            CreateMap<AnswerSurvay, AnswerSurvayDto>().PreserveReferences();
            CreateMap<AnswerSurvayDto, AnswerSurvay>().PreserveReferences();

            CreateMap<AnswerField, AnswerFieldDto>().PreserveReferences();
            CreateMap<AnswerFieldDto, AnswerField>().PreserveReferences();

            CreateMap<Client, ClientDto>().PreserveReferences();
            CreateMap<ClientDto, Client>().PreserveReferences();

            CreateMap<UserSurvay, UserSurvayDto>().PreserveReferences();
            CreateMap<UserSurvayDto, UserSurvay>().PreserveReferences();

            CreateMap<MatrixHeader, MatrixHeaderDto>().PreserveReferences();
            CreateMap<MatrixHeaderDto, MatrixHeader>().PreserveReferences();

            CreateMap<MatrixRow, MatrixRowDto>().PreserveReferences();
            CreateMap<MatrixRowDto, MatrixRow>().PreserveReferences();

            CreateMap<MatrixAnswer, MatrixAnswerDto>().PreserveReferences();
            CreateMap<MatrixAnswerDto, MatrixAnswer>().PreserveReferences();
        }
    }


}
