using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.CouponAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/coupon")]
    public class CouponApiController : Controller
    {
        private readonly ICouponRepository _couponRepository;
        private ResponseDto _response;

        public CouponApiController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
            this._response = new ResponseDto();
        }
        
        [HttpGet("{couponCode}")]
        public async Task<object> GetDiscountByCode(string couponCode)
        {
            try
            {
                var couponDto = await _couponRepository.GetCouponByCode(couponCode);
                _response.Result = couponDto;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }
    }
}