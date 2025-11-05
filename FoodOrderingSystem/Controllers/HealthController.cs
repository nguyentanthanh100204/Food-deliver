using System.Web.Mvc;

namespace FoodOrderingSystem.Controllers
{
    public class HealthController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ContentResult Index()
        {
            // Không đụng DB hay dịch vụ nào khác -> Chỉ để kiểm tra app còn “sống”
            return Content("OK");
        }
    }
}
