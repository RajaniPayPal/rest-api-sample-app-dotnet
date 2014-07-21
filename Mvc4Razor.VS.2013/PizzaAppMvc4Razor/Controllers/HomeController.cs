using System.Web.Mvc;

namespace PizzaAppMvc4Razor
{
    public class HomeController : Controller
    {
        #region ActionResult
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}
