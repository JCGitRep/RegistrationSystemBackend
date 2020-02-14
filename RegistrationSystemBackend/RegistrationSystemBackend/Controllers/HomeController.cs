using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace RegistrationSystemBackend.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class HomeController : Controller
    {
        private UsersEntities User_Entities = new UsersEntities();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        
    }
}
