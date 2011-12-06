namespace Nice.Ukps.Features.ShowHomePage.Services
{
    public interface IHome
    {
        Resources.Home GetResource();
    }

    public class Home : IHome
    {
        public Resources.Home GetResource()
        {
            return new Resources.Home();
        }

    }
}