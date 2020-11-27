using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Data()
        {
            return View(new DataViewModel
            {
                Data = await _apiService.GetApiDataAsync()
            });
        }

        public async Task<IActionResult> DataPost()
        {
            var isOk = await _apiService.PostApiDataAsync();

            return View(isOk);
        }

        public async Task<IActionResult> Users()
        {
            return View(new DataViewModel
            {
                Data = await _apiService.GetApiDataUsersAsync()
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
