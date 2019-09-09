﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace kudvenkat.Controllers {
    public class ErrorController : Controller {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger) {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode) {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode) {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you are looking for was not found.";
                    _logger.LogWarning($"404 Error occured. Path = {statusCodeResult.OriginalPath}{statusCodeResult.OriginalQueryString}");
                    return View("NotFound");
            }
            return View("NotFound");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error() {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            _logger.LogError($"The path {exceptionDetails.Path} threw an exception {exceptionDetails.Error}");

            return View("Error");
        }
    }
}