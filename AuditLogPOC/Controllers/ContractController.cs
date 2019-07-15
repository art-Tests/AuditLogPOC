using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuditLogPOC.Controllers
{
    public class ContractController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Log()
        {
            return View();
        }
    }
}