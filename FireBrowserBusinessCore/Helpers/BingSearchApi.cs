using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowserExceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FireBrowserBusinessCore.Helpers
{
    public partial class BingSearchApi : ObservableObject
    {
        /*  TODO: 
            //2024-02-20
            //TODO: add bing search api for trending to newTab.  
            // I've addes  a free bing search api subscription to my azure account 
            // key:  29948d69f0294a5a9b8b75831dd06c8a
            // curl -H "Ocp-Apim-Subscription-Key: 29948d69f0294a5a9b8b75831dd06c8a" https://api.bing.microsoft.com/v7.0/news/trendingtopics
            // make a variable $get
            // 1. $get = (Invoke-WebRequest -Uri "https://api.bing.microsoft.com/v7.0/news/trendingtopics?mkt=en-us" -Method Get -Headers @{ 'Ocp-Apim-Subscription-Key' = '29948d69f0294a5a9b8b75831dd06c8a' })
            // 2. $get is JsonObject 
            // 3. echo $get.Content 
            // 4. now lets parse it 
            // 5. $array = ($get.Content | ConvertFrom-Json) 
            // parse it by a pipe |=> convertfrom-json
            // 6. let see echo $array.value
            // 7. now what ok, lets convet to html 
            // 8. $array.value | ConvertTo-Html 
            // 9. save to a file 
            // 10. New-Item -Type File -Name "trending.html"
            // 11. Add-Content "trending.html" -Value ($array.value | ConvertTo-Html)
            // 12. Notepad .\trending.html 

            //<summary>
            //https://learn.microsoft.com/en-us/bing/search-apis/bing-web-search/tutorial/csharp-ranking-tutorial
            //https://learn.microsoft.com/en-us/bing/search-apis/bing-news-search/overview
            //https://stackoverflow.com/questions/70669970/how-to-hand-headers-in-curl-request-in-powershell-windows
            //https://stackoverflow.com/questions/50860411/read-array-from-json-and-pass-to-variable-in-powershell 
        */
        [ObservableProperty]
        private string _SearchQuery;
        List<JToken> TrendingList { get; set; }


        public BingSearchApi()
        {
            // result Jobject 
            // todo parse and return to HomeViewModel to add to newTab.. 
            TrendingList = [.. RunQueryAndDisplayResults(null).GetAwaiter().GetResult()];

        }

        public Task<List<JToken>> RunQueryAndDisplayResults(string userQuery)
        {
            try
            {
                // Create a query
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "29948d69f0294a5a9b8b75831dd06c8a");
                //<summary>
                //var queryString = HttpUtility.ParseQueryString(string.Empty);
                //queryString["q"] = userQuery;
                //var query = "https://api.bing.microsoft.com/v7.0/search?" + queryString;
                //</summary> user for a bing search 

                var query = $"https://api.bing.microsoft.com/v7.0/news/trendingtopics?mkt=en-us";
                // Run the query
                HttpResponseMessage httpResponseMessage = client.GetAsync(query).Result;

                // Deserialize the response content
                var responseContentString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                Newtonsoft.Json.Linq.JObject responseObjects = Newtonsoft.Json.Linq.JObject.Parse(responseContentString);

                //Handle success and error codes
                //if (httpResponseMessage.IsSuccessStatusCode)
                //{
                //    DisplayAllRankedResults(responseObjects);
                //}
                //else
                //{
                //    Console.WriteLine($"HTTP error status code: {httpResponseMessage.StatusCode.ToString()}");
                //}
                return Task.FromResult(responseObjects.SelectToken("value").ToList());
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);

            }

            return Task.FromResult<List<JToken>>(null);
        }
        static void DisplayAllRankedResults(Newtonsoft.Json.Linq.JObject responseObjects)
        {
            string[] rankingGroups = new string[] { "pole", "mainline", "sidebar" };

            // Loop through the ranking groups in priority order
            foreach (string rankingName in rankingGroups)
            {
                Newtonsoft.Json.Linq.JToken rankingResponseItems = responseObjects.SelectToken($"rankingResponse.{rankingName}.items");
                if (rankingResponseItems != null)
                {
                    foreach (Newtonsoft.Json.Linq.JObject rankingResponseItem in rankingResponseItems)
                    {
                        Newtonsoft.Json.Linq.JToken resultIndex;
                        rankingResponseItem.TryGetValue("resultIndex", out resultIndex);
                        var answerType = rankingResponseItem.Value<string>("answerType");
                        switch (answerType)
                        {
                            case "WebPages":
                                DisplaySpecificResults(resultIndex, responseObjects.SelectToken("webPages.value"), "WebPage", "name", "url", "displayUrl", "snippet");
                                break;
                            case "News":
                                DisplaySpecificResults(resultIndex, responseObjects.SelectToken("news.value"), "News", "name", "url", "description");
                                break;
                            case "Images":
                                DisplaySpecificResults(resultIndex, responseObjects.SelectToken("images.value"), "Image", "thumbnailUrl");
                                break;
                            case "Videos":
                                DisplaySpecificResults(resultIndex, responseObjects.SelectToken("videos.value"), "Video", "embedHtml");
                                break;
                            case "RelatedSearches":
                                DisplaySpecificResults(resultIndex, responseObjects.SelectToken("relatedSearches.value"), "RelatedSearch", "displayText", "webSearchUrl");
                                break;
                        }
                    }
                }
            }
        }
        static void DisplaySpecificResults(Newtonsoft.Json.Linq.JToken resultIndex, Newtonsoft.Json.Linq.JToken items, string title, params string[] fields)
        {
            if (resultIndex == null)
            {
                foreach (Newtonsoft.Json.Linq.JToken item in items)
                {
                    DisplayItem(item, title, fields);
                }
            }
            else
            {
                DisplayItem(items.ElementAt((int)resultIndex), title, fields);
            }
        }

        static void DisplayItem(Newtonsoft.Json.Linq.JToken item, string title, string[] fields)
        {
            Console.WriteLine($"{title}: ");
            foreach (string field in fields)
            {
                Console.WriteLine($"- {field}: {item[field]}");
            }
            Console.WriteLine();
        }
    }
}
