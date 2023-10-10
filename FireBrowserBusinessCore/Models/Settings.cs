namespace FireBrowserBusinessCore.Models;

public class Settings
{

    public enum UILayout
    {
        Modern,
        Vertical
    }

    public enum NewTabLayout
    {
        Classic,
        Simple,
        Productive
    }


    public enum NewTabBackground
    {
        None,
        Featured,
        Costum, //Bing for now, in the future Unsplash or our own service
    }
}
