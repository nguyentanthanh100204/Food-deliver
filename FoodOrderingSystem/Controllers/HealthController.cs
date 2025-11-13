using System.Net;
using System.Web.Mvc;

public class HealthController : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult Index()
    {
        // Trả về HTTP 200 đúng như test mong đợi
        return new HttpStatusCodeResult(HttpStatusCode.OK);
    }
}
