using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GG.Models;

namespace GG.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // *** Tillagt för CustomValidation NB ***
        [HttpPost]  
        [ValidateAntiForgeryToken]  
        public IActionResult New(AuctionItem item)  
        {  
            if (ModelState.IsValid)  
            {  
                RedirectToAction("Index");  
            }  
            return View();  
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
