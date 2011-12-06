using System.Web.Routing;
using Nice.Ukps.Features.HorizonScanner;
using Nice.Ukps.Features.ShowHomePage;
using Snooze.Routing;

namespace Nice.Ukps
{
    public class RouteRegistration : IRouteRegistration
    {
        public void Register(RouteCollection routes)
        {
            routes.AddIE6Support();

            routes.Map<HomeController.HomeUrl>(u => "");
            routes.Map<PharmaceuticalController.PharmaceuticalAddUrl>(u => "pharma/");
        }
    }
}