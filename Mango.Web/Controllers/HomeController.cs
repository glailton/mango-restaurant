using System;
using System.Collections.Generic;
using Mango.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using Mango.Services.ProductAPI.Models.Dtos;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> productDtos = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (response != null && response.IsSuccess)
            {
                productDtos = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result)!);
            }
            return View(productDtos);
        }
        
        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            var productDto = new ProductDto();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId,"");
            if (response != null && response.IsSuccess)
            {
                productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result)!);
            }
            return View(productDto);
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
        
        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
