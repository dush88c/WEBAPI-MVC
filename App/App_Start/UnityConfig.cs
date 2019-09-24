using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using SurvayArm.Application.IService;
using SurvayArm.Application.Service;
using SurvayArm.Application.Unity;
using Microsoft.AspNet.Identity;
using App.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using App.Controllers;
using App.Automapper;
using AutoMapper;

namespace App
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            var mapper = MappingProfile.InitializeAutoMapper().CreateMapper();
            container.RegisterInstance<IMapper>(mapper);
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<ISurvayService , SurvayService>();
            container.RegisterType<ISurvayTypeService , SurvayTypeService>();
            container.RegisterType<IFieldDependantService, FieldDependantService>();
            container.RegisterType<IFieldService, FieldService>();
            container.RegisterType<IFieldOptionService, FieldOptionService>();
            container.RegisterType<IOptionService, OptionService>();
            container.RegisterType<IProvinceService, ProvinceService>();
            container.RegisterType<IDistricService, DistricService>();
            container.RegisterType<ISurvaySettingService, SurvaySettingService>();
            container.RegisterType<ISurvaySupervisorService, SurvaySueprvisorService>();
            container.RegisterType<ISurvayTargetService, SurvayTargetService>();
            container.RegisterType<IDeviceManagerService, DeviceManagerService>();
            container.RegisterType<IAnswerSurvayService, AnswerSurvayService>();
            container.RegisterType<IClientService, ClientService>();
            container.RegisterType<IUserSurvayService, UserSurvayService>();
            container.RegisterType<IMatrixHeaderService, MatrixHeaderService>();
            container.RegisterType<IMatrixRowService, MatrixRowService>();

            //Data Layer dependency mapping as extension eg : IUnitOfWork
            container.AddNewExtension<DependencyInjectionExtension>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}