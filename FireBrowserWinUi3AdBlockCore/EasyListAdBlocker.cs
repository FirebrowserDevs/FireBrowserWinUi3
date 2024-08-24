using DistillNET;
using FireBrowserWinUi3MultiCore;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FireBrowserWinUi3AdBlockCore.AdBlocker
{
    public class EasyListAdBlocker // 200ms/test * 60000/tests per requests and use 1gb of ram to inititalize
    {
        public class Filter
        {
            public Regex Regex { get; set; }
            public bool BlockIfNotTrue { get; set; } = false;
            public List<Option> Options { get; set; } = null;
            public Filter(Regex regex, bool blockIfNotTrue, List<Option> options)
            {
                Regex = regex;
                BlockIfNotTrue = blockIfNotTrue;
                Options = options;
            }
        }

        public class Option()
        {
            public string Context { get; set; } = string.Empty;
            public string DomainRestriction { get; set; } = null;
            public bool Inverse { get; set; } = false;
        }

        public static List<Filter> AdBlockList { get; set; } = new();



        public static void AttachCoreWebView2(CoreWebView2 coreWebView2)
        {
            coreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            coreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        }

        private static void CoreWebView2_WebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            if (AuthService.CurrentUser.UserSettings.IsAdBlockerEnabled)
            {
                Uri url = new Uri(args.Request.Uri);
                var headers = new NameValueCollection(StringComparer.OrdinalIgnoreCase)
                {
                    { "X-Requested-With", args.ResourceContext.ToString() },
                };

                foreach (var header in args.Request.Headers)
                {
                    var nvcol = new NameValueCollection(StringComparer.OrdinalIgnoreCase) { { header.Key, header.Value } };
                    headers.Add(nvcol);
                }

                //var filters = _filterCollection.GetFiltersForDomain(url.Host);
                //var whitelist = _filterCollection.GetWhitelistFiltersForDomain(url.Host);

               // bool matches = filters.Any(p => p.IsMatch(url, headers)) && !whitelist.Any(p => p.IsMatch(url, headers));

                //if (matches)
               // {
               //     sender.Stop();
                //    args.Response = sender.Environment.CreateWebResourceResponse(null, 503, "Service Unavailable", "");
                //    sender.Resume();
               // }
            }
        }


        public static async Task<bool> ShouldBlock(string str, string domain, CoreWebView2WebResourceContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            bool should = false;
            bool shouldnt = false;
            AdBlockList.AsParallel().ForAll(p =>
            {
                if (p.Regex.IsMatch(str)) should = true;
                if (should && p.BlockIfNotTrue) shouldnt = true;

                if (should && !shouldnt)
                {
                    foreach (var option in p.Options)
                    {
                        if (!string.IsNullOrEmpty(option.Context) && option.Context == context.ToString().ToLower() && !option.Inverse)
                        {
                            should = true;
                        }
                        else if (!string.IsNullOrEmpty(option.Context))
                        {
                            shouldnt = true;
                        }
                        else if (!string.IsNullOrEmpty(option.DomainRestriction) && domain.Contains(option.DomainRestriction) && !option.Inverse)
                        {
                            should = true;
                        }
                        else if (!string.IsNullOrEmpty(option.DomainRestriction))
                        {
                            shouldnt = true;
                        }


                    }
                }
            });

            await Task.Delay(1);

            watch.Stop();
            Debug.WriteLine($"Time : {watch.ElapsedMilliseconds}ms, Blocked : {should && !shouldnt}");
            return should && !shouldnt;
        }

        public static void CreateRegexBasedFilterList()
        {
            string assetsFile = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "FireBrowserWinUi3AdBlockCore.AdBlocker", "Assets");
            string easylistPath = Path.Combine(assetsFile, "easylist_general_block.txt");

            string[] text = File.ReadAllLines(easylistPath);
            List<Filter> regexes = new();

            foreach (string filter in text)
            {
                string regex = filter.Contains("$") ? filter.Split("$")[0] : filter;
                bool blockIfNotTrue = false;
                List<Option> options = new();

                regex = regex.Replace("||", "https{0,1}")
                     .Replace("*", ".*")
                     .Replace("^", @"[/:\?\=&]");

                if (regex[0] == '|') regex = string.Concat("^", regex.AsSpan(1));
                if (regex.EndsWith('|')) regex = regex.Remove(regex.Length - 1, 1) + "$";

                if (regex.StartsWith("@@")) { regex = regex.Replace("@@", ""); blockIfNotTrue = true; }

                if (filter.Contains("$"))
                {
                    string opts = filter.Split("$")[1];
                    string[] optArray = opts.Contains(',') ? opts.Split(",") : [opts];

                    foreach (string opt in optArray)
                    {
                        Option option = new();
                        if (opt.Contains("domain="))
                        {
                            string domain = opt.Split("=")[1];
                            string domainWithoutInv = domain.Replace("~", "");

                            if (domainWithoutInv != domain) option.Inverse = true;

                            option.DomainRestriction = domainWithoutInv;
                        }
                        else
                        {
                            string optWithoutInv = opt.Replace("~", "");

                            if (optWithoutInv != opt) option.Inverse = true;

                            option.Context = optWithoutInv;
                        }

                        options.Add(option);
                    }
                }


                try
                {
                    Regex reg = new(regex, RegexOptions.NonBacktracking | RegexOptions.Compiled, new TimeSpan(0, 0, 2));
                    regexes.Insert(0, new Filter(reg, blockIfNotTrue, options));
                }
                catch
                {
                    Regex reg = new(Regex.Escape(regex), RegexOptions.NonBacktracking | RegexOptions.Compiled, new TimeSpan(0, 0, 2));
                    regexes.Insert(0, new Filter(reg, blockIfNotTrue, options));
                }
            }

            AdBlockList = regexes;
        }
    }
}