using System;
using System.ComponentModel.Composition.Hosting;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Nice.Ukps.Spec.contexts
{
    public static class document_store
    {
        public static EmbeddableDocumentStore GetDocumentStore(Type clientIndex)
        {
            var store = GetDocumentStore();
            var catalogue = new TypeCatalog(new[] { clientIndex });

            IndexCreation.CreateIndexes(new CompositionContainer(catalogue), store);
            return store;
        }

        public static EmbeddableDocumentStore GetDocumentStore(bool inmem = true)
        {
            var store = new EmbeddableDocumentStore
            {
                Configuration =
                {
                    RunInMemory = inmem
                }
            };

            store.Initialize();
            
            return store;
        }
    }
}