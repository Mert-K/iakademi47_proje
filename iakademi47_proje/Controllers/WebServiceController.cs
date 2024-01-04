using Microsoft.AspNetCore.Mvc;

namespace iakademi47_proje.Controllers
{
    public class WebServiceController : Controller
    {
        //e fatura için

        public static string tckimlikno = string.Empty;
        public static string vergino = string.Empty;
        public IActionResult Index()
        {
            return View();
        }
    }
}
