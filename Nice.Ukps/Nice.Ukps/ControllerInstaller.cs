using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Snooze;

namespace Nice.Ukps
{
    public class ControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromThisAssembly()
                 .Pick()
                 .If(t => typeof(ResourceController).IsAssignableFrom(t))
                 .Configure(c => c.LifeStyle.HybridPerWebRequestTransient())
                 .WithService.Self());
        }
    }
}