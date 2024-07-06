using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Web.Http;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace FireBrowserWinUi3Core;

public class BingFetch
{
    private string _strRegion = "en-US";
    private int _numOfImages = 1;
    private string _strBingImageURL;
    public string _strJSONString = "";
    public List<string> _lstBingImageURLs = new List<string>();

    public BingFetch(int numOfImages = 1, string region = "en-US")
    {
        _numOfImages = numOfImages;
        _strRegion = region;
        _strBingImageURL = $"http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n={_numOfImages}&mkt={_strRegion}";
    }

    public async Task RetrieveBingImageJsonAsync()
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(new Uri(_strBingImageURL));
        _strJSONString = await response.Content.ReadAsStringAsync();
    }

    public void ParseJsonUsingWindowsDataJson()
    {
        JsonObject jsonObject;
        bool boolParsed = JsonObject.TryParse(_strJSONString, out jsonObject);

        if (boolParsed)
        {
            for (int i = 0; i < _numOfImages; i++)
            {
                _lstBingImageURLs.Add(jsonObject["images"].GetArray()[i].GetObject()["url"].GetString());
            }
        }
    }
}
