using Mango.Services.ProductAPI.Models.Dtos;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        protected ResponseDto _response;
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            this._response = new ResponseDto();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _productRepository.GetProducts();
                _response.Result = productDtos;

            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpGet]
        [Route("{productId}")]
        public async Task<object> GetById(int productId)
        {
            try
            {
                var productDto = await _productRepository.GetProductById(productId);
                _response.Result = productDto;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpPost]
        public async Task<object> Post([FromBody] ProductDto productDto)
        {
            try
            {
                var model = await _productRepository.CreateUpdateProduct(productDto);
                _response.Result = model;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

        [HttpPut]
        public async Task<object> Put([FromBody] ProductDto productDto)
        {
            try
            {
                var model = await _productRepository.CreateUpdateProduct(productDto);
                _response.Result = model;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }


        [HttpDelete]
        [Route("{productId}")]
        public async Task<object> Put(int productId)
        {
            try
            {
                var result = await _productRepository.DeleteProduct(productId);
                _response.Result = result;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }

            return _response;
        }

    }
}
