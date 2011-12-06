using System.Web;
using System.Web.Routing;
using Castle.Windsor;

namespace Nice.Ukps
{
    public static class ServiceLocator
    {
        public static T Resolve<T>()
        {
            return MvcApplication.Container.Resolve<T>();
        }
    }

    public class MvcApplication : HttpApplication
    {
        public static IWindsorContainer Container { get; set; }

        protected void Application_Start()
        {
            CreateContainer();
            StartApplication();
        }

        protected void Application_End()
        {
            EndApplication();
        }

        public IWindsorContainer CreateContainer()
        {
            Container = new WindsorContainer();
            return Container;
        }

        public virtual void StartApplication() { }

        public virtual void EndApplication() { }

        public virtual void RegisterRoutes(RouteCollection routes) { }
    }
}