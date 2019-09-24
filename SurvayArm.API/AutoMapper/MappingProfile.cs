using AutoMapper;
using SurvayArm.Application.Automapper;


namespace SurvayArm.API.AutoMapper
{
    public static class MappingProfile
    {
        public static MapperConfiguration InitializeAutoMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new WebMappingProfile());  //mapping between Web and Business layer objects
                cfg.AddProfile(new BlProfile());  // mapping between Business and DB layer objects
            });

            return config;
        }
    }
}