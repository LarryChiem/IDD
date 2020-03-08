﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace mvc_trial.Controllers
{
    public class AdminController : Controller
    {
        // GET: /Admin/Dashboard
        [HttpPost("Admin")]
        public IActionResult Dashboard()
        {
            // Create string from header values
            string headers = String.Empty;
            foreach(var key in Request.Headers.Keys)
            {
                headers += key + " -> " + Request.Headers[key] + "\n";
            }
            System.Diagnostics.Debug.WriteLine(headers);
            ViewData["Headers"] = headers;
            return View();
        }

        // GET /Admin/filterDate/?start_date=&end_date=
        public IActionResult FilterDate(string start_date, string end_date)
        {
            return Json(new {
                results=$"Nothing found from {start_date} to {end_date}"
            });
        }

        // GET /Admin/filtername/?first_name=&last_name=
        public IActionResult FilterName(string first_name, string last_name)
        {
            return Json(new {
                results=$"Nothing found for {last_name},{first_name}"
            });
        }
    }
}
