using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Controllers {
    public class DepartmentsController : Controller {
        public DepartmentsController() {

        }

        public string List() {
            return "List() of DepartmentController";
        }

        public string Details() {
            return "Details() of DepartmentController";
        }

    }
}
