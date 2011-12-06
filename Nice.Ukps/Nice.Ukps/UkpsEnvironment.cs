using System;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace Nice.Ukps
{
    public enum Mode
    {
        Development,
        Test,
        Authoring,
        Live,
        ReleaseCandidate
    }

    public class UkpsEnvironment
    {
        public Mode Mode { get; set; }

        public static UkpsEnvironment Current
        {
            get
            {
                return ServiceLocator.Resolve<UkpsEnvironment>();
            }
        }

        public string RavenFolder
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\Raven"; }
        }
        public EmbeddableDocumentStore DocumentStore()
        {
            return (Mode == Mode.Test || Mode == Mode.Development) ?
                InMemoryStore() :
                FileSystemStore();
        }


        EmbeddableDocumentStore FileSystemStore()
        {
            return new EmbeddableDocumentStore
            {
                Configuration =
                {
                    DefaultStorageTypeName = "Esent",
                    Port = 3137,
                    AnonymousUserAccessMode = AnonymousUserAccessMode.All
                },
                UseEmbeddedHttpServer = false,
                DataDirectory = RavenFolder
            };
        }

        EmbeddableDocumentStore InMemoryStore()
        {
            return new EmbeddableDocumentStore
            {
                Configuration = { Port = 3137, AnonymousUserAccessMode = AnonymousUserAccessMode.All },
                UseEmbeddedHttpServer = true,
                RunInMemory = true
            };
        }
    }
}