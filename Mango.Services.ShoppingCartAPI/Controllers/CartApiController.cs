using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartApiController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMessageBus _messageBus;
        private ResponseDto _response;

        public CartApiController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository)
        {
            _cartRepository = cartRepository;
            _messageBus = messageBus;
            _couponRepository = couponRepository;
            this._response = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }
        
        [HttpPost("AddCart")]
        public async Task<object> AddCart([FromBody] CartDto cartDto)
        {
            try
            {
                CartDto cartDtoFromDb = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cartDtoFromDb;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }
        
        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart([FromBody] CartDto cartDto)
        {
            try
            {
                CartDto cartDtoFromDb = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cartDtoFromDb;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }
        
        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                Boolean isSuccess = await _cartRepository.RemoveFromCart(cartDetailsId);
                _response.Result = isSuccess;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }
        
        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                Boolean isSuccess = await _cartRepository
                    .ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                _response.Result = isSuccess;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }
        
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                Boolean isSuccess = await _cartRepository
                    .RemoveCoupon(userId);
                _response.Result = isSuccess;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { e.ToString() };
            }

            return _response;
        }
        
        [HttpPost("Checkout")]
        public async Task<object> Checkout([FromBody] CheckoutHeaderDto checkoutHeaderDto)
        {
            try
            {
                CartDto cartDtoFromDb = await _cartRepository.GetCartByUserId(checkoutHeaderDto.UserId);
                if (cartDtoFromDb == null)
                {
                    return BadRequest();
                }

                if (!string.IsNullOrEmpty(checkoutHeaderDto.CouponCode))
                {
                    CouponDto couponDto = await _couponRepository.GetCoupon(checkoutHeaderDto.CouponCode);
                    if (checkoutHeaderDto.DiscountTotal != couponDto.DiscountAmount)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>() { "Coupon Price has changed, please confirm" };
                        _response.DisplayMessage = "Coupon Price has changed, please confirm";
                        return _response;
                    }
                }

                checkoutHeaderDto.CartDetails = cartDtoFromDb.CartDetails;
                
                //logic to add message to process order
                await _messageBus.PublishMessage(checkoutHeaderDto, "checkoutqueue");
                await _cartRepository.ClearCart(checkoutHeaderDto.UserId);
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