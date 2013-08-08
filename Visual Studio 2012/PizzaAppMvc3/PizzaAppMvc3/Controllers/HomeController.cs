using System.Web.Mvc;

namespace PizzaAppMvc3
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
    }
}
