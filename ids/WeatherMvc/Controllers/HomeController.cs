﻿using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WeatherMvc.Models;
using WeatherMvc.Services;

namespace WeatherMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ITokenService tokenService, ILogger<HomeController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Weather()
        {
            var data = new List<WeatherData>();
            using (var client = new HttpClient())
            {
                var tokenResponse = await _tokenService.GetToken("weatherapi.read");

                client.SetBearerToken(tokenResponse.AccessToken);
                
                var result = client.GetAsync("https://localhost:5445/weatherforecast").Result;

                if (result.IsSuccessStatusCode)
                {
                    var model = result.Content.ReadAsStringAsync().Result;

                    data = JsonConvert.DeserializeObject<List<WeatherData>>(model);

                    return View(data);
                }
                else
                {
                    throw new Exception("Unable to get content");
                }
            }
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}