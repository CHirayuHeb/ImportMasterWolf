using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ImportMasterWolf.Controllers
{
    public class ErrorCaseController : Controller
    {
        public PartialViewResult Index()
        {
            return PartialView("c403");
        }
    }
}