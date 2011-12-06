using Glue;
using Raven.Client;

namespace Nice.Ukps.Features.Pharmaceutical.Services
{
    public interface IAmPharmaceutical
    {
        Resources.Pharmaceutical GetResource();
        Resources.Pharmaceutical GetResource(string id);
        Resources.Pharmaceutical PutResource(Resources.Pharmaceutical input);
    }
    public class Pharmaceutical : IAmPharmaceutical
    {
        protected readonly IDocumentSession raven;

        public Pharmaceutical(IDocumentSession raven)
        {
            this.raven = raven;
        }

        public Resources.Pharmaceutical GetResource()
        {
            return new Resources.Pharmaceutical();
        }

        public Resources.Pharmaceutical GetResource(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            return new Mapping<Persistable.Technology, Resources.Pharmaceutical>().Map(raven.Load<Persistable.Technology>(id));
        }

        public Resources.Pharmaceutical PutResource(Resources.Pharmaceutical input)
        {
            // get the current state out of the database
            var get = GetResource(input.Id);

            // compare with new state

            var put = new Mapping<Resources.Pharmaceutical, Persistable.Technology>().Map(input);
            raven.Store(put);
            raven.SaveChanges();
            input.Id = put.Id;
            return input;
        }
    }
}