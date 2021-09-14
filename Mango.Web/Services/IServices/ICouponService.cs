using Mango.Web.Models;
using System.Threading.Tasks;

namespace Mango.Web.Services.IServices
{
    public interface ICouponService : IBaseService
    {
        Task<T> GetCoupon<T>(string couponCode, string token = null);
    }
}
