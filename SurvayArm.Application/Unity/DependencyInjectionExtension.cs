using Microsoft.Practices.Unity;
using SurvayArm.Data.UOW;

namespace SurvayArm.Application.Unity
{
   public class DependencyInjectionExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IUnitOfWork, UnitOfWork>();
        }
    }
}
