using Glue;
using Raven.Client;
using Snooze;

namespace Nice.Ukps.Features.NHS.Services
{
    public class NHS
    {
        protected readonly IDocumentSession raven;
        public Resources.NHS GetTechnology(string id)
        {

            if (string.IsNullOrEmpty(id))
                return null;
            return new Mapping<Persistable.Technology, Resources.NHS>().Map(raven.Load<Persistable.Technology>(id));
        }
    }
}