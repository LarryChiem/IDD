using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminUI.Data;
using AdminUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        // GET: PayPeriod
        public ActionResult Index()
        {
            return View(_context.PayPeriods.AsEnumerable());
        }

        // GET: PayPeriod/Details/5
        public ActionResult StartPayPeriod(int id)
        {
            var pps = _context.PayPeriods.Where(p => p.Current);
            foreach (var pp in pps)
            {
                pp.Current = false;
                _context.Update(pp);
            }
            var payPeriod = _context.PayPeriods.Find(id);
            payPeriod.Current = true;
            GlobalVariables.CurrentPayPeriod = payPeriod;
            _context.Update(payPeriod);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: PayPeriod/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PayPeriod/Create
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

        // GET: PayPeriod/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_context.PayPeriods.Find(id));
        }

        // POST: PayPeriod/Edit/5
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

        // GET: PayPeriod/Delete/5
        public ActionResult Delete(int id)
        {
            var pp = _context.PayPeriods.Find(id);
            if (pp.Current)
                return RedirectToAction(nameof(Index));
            return View(pp);
        }

        // POST: PayPeriod/Delete/5
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