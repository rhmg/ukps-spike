using Nice.Ukps.Features.ShowHomePage.Services;
using Snooze;

namespace Nice.Ukps.Features.ShowHomePage
{
    public class HomeController : ResourceController
    {
        public class HomeUrl : Url { }

        readonly IHome home;

        public HomeController(IHome home)
        {
            this.home = home;
        }

        public ResourceResult Get(HomeUrl url)
        {
            return OK(home.GetResource())
                .AsHtml();
        }
    }
}