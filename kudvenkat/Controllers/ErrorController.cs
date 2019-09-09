using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace kudvenkat.Controllers {
    public class ErrorController : Controller {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode) {
            switch (statusCode) {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you are looking for was not found.";
                    return View("NotFound");
                    break;
            }
            return View("NotFound");
        }
    }
}