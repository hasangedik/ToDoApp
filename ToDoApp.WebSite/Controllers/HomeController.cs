using System.Web.Mvc;

namespace ToDoApp.WebSite.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}