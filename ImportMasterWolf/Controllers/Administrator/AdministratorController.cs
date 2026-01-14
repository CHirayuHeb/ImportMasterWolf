using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ImportMasterWolf.Controllers.Administrator
{
    public class AdministratorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult ApproverSetting()
        {
            return PartialView("~/Views/Administrator/Setting/_ptvApproverSetting.cshtml");
        }
    }
}