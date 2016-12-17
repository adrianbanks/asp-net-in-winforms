using System;
using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Models;

namespace MyWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["time"] = DateTime.Now.ToString();
            return View();
        }

        [HttpPost]
        public IActionResult TestSubmit(SubmitModel model)
        {
            var message = "You typed " + model.Message;
            ViewData["message"] = message;
            Program.MessageBox(message);
            return View();
        }
    }
}
