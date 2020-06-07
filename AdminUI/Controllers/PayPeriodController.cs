using System;
using System.Linq;
using AdminUI.Data;
using AdminUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminUI.Controllers
{
    [Authorize(Roles="Administrator")]
    public class PayPeriodController : Controller
    {
        private readonly PayPeriodContext _context;

        public PayPeriodController(PayPeriodContext context)
        {
            _context = context;
        }
        /*
         * The Index of all Pay Periods
         * Parameters: None
         * Returns the Pay Period Index
         */
        public ActionResult Index()
        {
            return View(_context.PayPeriods.AsEnumerable());
        }

        /*
         * Assigns the given pay period as the Global Current Pay Period
         * Parameters: ID of the pay period
         * Returns the Pay Period Index
         */
        public ActionResult StartPayPeriod(int id)
        {
            //Gets all Pay Periods labeled as the current one. Should only one return 1, but better safe than sorry
            var pps = _context.PayPeriods.Where(p => p.Current);
            foreach (var pp in pps)
            {
                //set it to false and update it
                pp.Current = false;
                _context.Update(pp);
            }
            //now find the one we want to start
            var payPeriod = _context.PayPeriods.Find(id);

            //set it to true and update the global variable accordingly
            payPeriod.Current = true;
            GlobalVariables.CurrentPayPeriod = payPeriod;
            _context.Update(payPeriod);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        /*
         * Returns the Create View
         * Parameters: None
         * Returns the Create View
         */
        public ActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Pay Period
         * Parameters: A dateFrom and a dateTo
         * Returns the Pay Period Index
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                _context.Add(new PayPeriod {DateFrom = dateFrom, DateTo = dateTo});
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        /*
         * Returns the Edit View
         * Parameters: the Pay Period ID
         * Returns the Edit View
         */
        public ActionResult Edit(int id)
        {
            return View(_context.PayPeriods.Find(id));
        }

        /*
         * Updates a Pay Period
         * Parameters: The Pay Period ID and the dateTo and dateFrom
         * Returns the Pay Period Index
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, DateTime dateTo, DateTime dateFrom)
        {
            var pp = _context.PayPeriods.Find(id);
            try
            {
                pp.DateFrom = dateFrom;
                pp.DateTo = dateTo;
                if (pp.Current)
                    GlobalVariables.CurrentPayPeriod = pp;
                _context.Update(pp);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(pp);
            }
        }

        /*
         * Returns the Delete View
         * Parameters: The Pay Period ID
         * Returns the Delete View
         */
        public ActionResult Delete(int id)
        {
            var pp = _context.PayPeriods.Find(id);
            if (pp.Current)
                return RedirectToAction(nameof(Index));
            return View(pp);
        }

        /*
         * Deletes a Pay Period. Fails if the pay period is the current one
         * Parameters: The Pay Period ID
         * Returns the Pay Period Index
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            var pp = _context.PayPeriods.Find(id);
            if (pp.Current)
                return RedirectToAction(nameof(Index));
            try
            {
                _context.Remove(pp);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Delete", pp);
            }
        }
    }
}