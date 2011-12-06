using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Snooze;
using Snooze.Routing;
using Spark;
using Spark.FileSystem;
using Spark.Web.Mvc;

namespace Nice.Ukps
{
    public class UkpsApplication : MvcApplication
    {
        public override void RegisterRoutes(RouteCollection routes)
        {
            routes.FromAssemblyWithType<RouteRegistration>();
        }

        public override void StartApplication()
        {
            ConfigureContainer();
            ConfigureSpark();

            RegisterRoutes(RouteTable.Routes);

        }

        public void ConfigureContainer()
        {

            Container.Install(new IWindsorInstaller[]
                                  {
                                      new UkpsInstaller(),
                                      new ControllerInstaller()
                                });

        }

        public void ConfigureSpark()
        {
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(Container));

            var settings = GetConventionFolderSettings();
            settings.SetAutomaticEncoding(true);

            SparkEngineStarter.RegisterViewEngine(settings);

            if (Properties.Settings.Default.PrecompileSpark)
            {
                var viewFactory = new SparkViewFactory(settings);
                var batch = new SparkBatchDescriptor();

                var controllerTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => typeof(ResourceController).IsAssignableFrom(type));

                foreach (var controllerType in controllerTypes)
                {
                    batch.For(controllerType);
                }
                viewFactory.Precompile(batch);
            }
        }

        SparkSettings GetConventionFolderSettings()
        {
            var settings = new SparkSettings();

            foreach (var dir in new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetDirectories("Views", SearchOption.AllDirectories))
            {
                settings.AddViewFolder(
                    ViewFolderType.FileSystem,
                    new Dictionary<string, string> { { "basePath", dir.FullName } });
            }

            return settings;
        }
    }
}