using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nice.UKPS.Properties;
using Raven.Client;
using Raven.Client.Indexes;

namespace Nice.Ukps
{
    public class UkpsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterEnvironment(container);

            RegisterRaven(container);

            container.Register(ImplementationsInNamespaceEndingWith("Services"));
        }

        static void RegisterEnvironment(IWindsorContainer container)
        {
            container.Register(Component.For<UkpsEnvironment>()
                .ImplementedBy<UkpsEnvironment>()
                .LifeStyle.Singleton
#pragma warning disable 612,618
.Parameters(Parameter.ForKey("Mode").Eq(Settings.Default.Mode.ToString())));
#pragma warning restore 612,618
        }


        static void RegisterRaven(IWindsorContainer container)
        {
            container.Register(Component.For<IDocumentStore>()
                                   .LifeStyle.Singleton
                                   .Instance(DocumentStore(container)));

            container.Register(Component.For<IDocumentSession>()
                    .LifeStyle.HybridPerWebRequestTransient()
                    .UsingFactoryMethod(c => c.Resolve<IDocumentStore>().OpenSession()));
        }

        static BasedOnDescriptor ImplementationsInNamespaceEndingWith(string ns)
        {
            return AllTypes.FromThisAssembly()
                .Pick()
                .If(t => t.Namespace != null && t.Namespace.EndsWith(ns))
                .Configure(c => c.LifeStyle.HybridPerWebRequestTransient())
                .WithService.AllInterfaces()
                .WithService.Self();
        }

        static IDocumentStore DocumentStore(IWindsorContainer container)
        {
            var store = container.Resolve<UkpsEnvironment>().DocumentStore();
            store.Configuration.Catalog.Catalogs.Add(new AssemblyCatalog(typeof(UkpsApplication).Assembly));
            store.Initialize();

            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), store);
            return store;
        }
    }
}
