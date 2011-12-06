using Nice.Ukps.Features.Pharmaceutical.Services;
using Snooze;

namespace Nice.Ukps.Features.HorizonScanner
{
    public class PharmaceuticalController : ResourceController
    {
        public class PharmaceuticalAddUrl : Url
        {
            public string Id { get; set; }
        }

        protected readonly IAmPharmaceutical scanner;
        public PharmaceuticalController(IAmPharmaceutical scanner)
        {
            this.scanner = scanner;
        }

        public ResourceResult Get(PharmaceuticalAddUrl addUrl)
        {
            return OK(scanner.GetResource());
        }

        public ResourceResult Post(PharmaceuticalAddUrl addUrl, Resources.Pharmaceutical input)
        {
            return OK(scanner.PutResource(input));
        }
    }
}