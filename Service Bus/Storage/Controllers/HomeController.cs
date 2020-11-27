using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperApp.Integration.TableStorage;
using SuperApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SuperApp.Controllers
{
    public class HomeController : Controller
    {
        private static Dictionary<string, string> OperatingSystems { get; } = new Dictionary<string, string>
            {
                { "windows", "Windows" },
                { "Mac", "MacOS" },
                { "x11", "Unix" },
                { "android", "Android" },
                { "iphone", "iOS" }
            };

        private static Dictionary<string, string> Browsers { get; } = new Dictionary<string, string>
            {
                { "msie", "Internet Explorer" },
                { "edg", "Microsoft Edge" },
                { "chrome", "Google Chrome" },
                { "safari", "Safari" },
                { "opr", "Opera" },
                { "opera", "Opera" },
                { "firefox", "Firefox" }
            };

        private ILogger<HomeController> Logger { get; }
        private StorageProxy StorageProxy { get; }

        public HomeController(ILogger<HomeController> logger, StorageProxy storageProxy)
        {
            Logger = logger;
            StorageProxy = storageProxy;
        }

        public async Task<IActionResult> Index()
        {
            string header = GetHeaderValue("User-Agent");
            string osName = GetOsName(header);
            string browserName = GetBrowserName(header);

            ClientResult result = await StorageProxy.GetClientResultAsync(osName, browserName);

            if (result == null)
            {
                result = new ClientResult
                {
                    BrowserName = browserName,
                    OsName = osName
                };
            }

            result.CombinationCount++;

            await StorageProxy.SaveClientResultAsync(result);

            result.CombinationCount--;

            return View(result);
        }

        private string GetHeaderValue(string headerName)
        {
            if (Request.Headers.TryGetValue(headerName, out var headerValue))
            {
                return headerValue;
            }

            return null;
        }

        private string GetOsName(string headerValue)
        {
            if (string.IsNullOrEmpty(headerValue))
            {
                return "UNKNOWN";
            }

            foreach (var item in OperatingSystems)
            {
                if (headerValue.IndexOf(item.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return item.Value;
                }
            }

            return $"UnKnown, More-Info: {headerValue}";
        }

        private string GetBrowserName(string headerValue)
        {
            if (string.IsNullOrEmpty(headerValue))
            {
                return "UNKNOWN";
            }

            foreach (var item in Browsers)
            {
                if (headerValue.IndexOf(item.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return item.Value;
                }
            }

            return $"UnKnown, More-Info: {headerValue}";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
