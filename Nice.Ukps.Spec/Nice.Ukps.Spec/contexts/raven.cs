using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Machine.Specifications;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Json.Linq;

namespace Nice.Ukps.Spec.contexts
{
    public class with_raven
    {
        public static EmbeddableDocumentStore store;
        static IDocumentSession s;

        public static IDocumentSession session
        {
            get { return s; }
        }

        public static void init()
        {
            if (store != null)
            {
                close();
            }
            store = document_store.GetDocumentStore();

            s = store.OpenSession();
        }

        public static void init_esent()
        {
            if (store != null)
            {
                close();
            }
            store = document_store.GetDocumentStore(false);

            s = store.OpenSession();
        }


        public static void close()
        {
            foreach (var error in store.DocumentDatabase.Statistics.Errors)
            {
                Console.WriteLine(@"Raven error on document {0} / index {1} {2}", error.Document, error.Index, error.Error);
            }
            store.Dispose();
            s.Dispose();
            store = null;
            s = null;
        }

        public static void store_documents<T>(IEnumerable<T> documents)
        {
            foreach (var document in documents)
                session.Store(document);

            session.SaveChanges();
        }

        public static void raven_contains<T>(IEnumerable<string> ids)
        {
            foreach (var id in ids)
                raven_contains<T>(id);
        }
        public static void raven_contains<T>(string id)
        {
            session.Advanced.Clear();
            session.Load<T>(id).ShouldNotBeNull();
        }

        public static T load_document<T>(string id)
        {
            session.Advanced.Clear();
            var ret = session.Load<T>(id);

            return ret;
        }

        public static RavenJObject load_metadata<T>(string id)
        {
            var doc = load_document<T>(id);
            return session.Advanced.GetMetadataFor(doc);
        }

        public static T store_document<T>(T document)
        {
            session.Advanced.Clear();
            session.Store(document);
            session.SaveChanges();

            return load_document<T>((string)((dynamic)document).Id);
        }

        public static void wait(Action a)
        {
            a();
            wait();
        }

        public static IEnumerable<T> index<T>(string name)
        {
            return session
                .Advanced.LuceneQuery<T>(name)
                .WaitForNonStaleResults()
                .ToArray();
        }

        public static EmbeddableDocumentStore wait()
        {

            store.DocumentDatabase.SpinBackgroundWorkers();

            while (store.DocumentDatabase.Statistics.StaleIndexes.Length > 0)
            {
                Thread.Sleep(50);
            }
            return store;
        }

        Establish raven = init;

        Cleanup cleanup_raven = close;

        public static void delete_document(string id)
        {
            session.Delete(session.Load<object>(id));
            session.SaveChanges();
            session.Advanced.Clear();
        }
    };

    public class service_with_raven_persistence<TService> : with_auto_mocking<TService> where TService : class
    {
        public static void store_documents<T>(IEnumerable<T> documents)
        {
            with_raven.store_documents(documents);
        }

        public static IQueryable<T> query<T>()
        {
            with_raven.session.Advanced.Clear();
            return with_raven.session
                .Query<T>();
        }

        public static void raven_contains<T>(string id)
        {
            with_raven.session.Advanced.Clear();
            with_raven.raven_contains<T>(id);
        }

        public static void raven_contains<T>(IEnumerable<string> ids)
        {
            with_raven.session.Advanced.Clear();
            with_raven.raven_contains<T>(ids);
        }

        public static T load_document<T>(string id)
        {
            with_raven.session.Advanced.Clear();
            var res = with_raven.load_document<T>(id);

            Console.WriteLine(res.ToString());

            return res;
        }

        public static void store_document<T>(T document)
        {
            with_raven.store_document(document);
        }

        public static void wait(Action a)
        {
            with_raven.wait(a);
        }

        Establish raven = () =>
        {
            with_raven.init();
            autoMocker.Inject(with_raven.session);
        };

        Cleanup cleanup_raven = with_raven.close;

        protected static TService Service { get { return autoMocker.ClassUnderTest; } }

    }
}