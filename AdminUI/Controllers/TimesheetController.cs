using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdminUI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdminUI.Controllers
{
    public class TimesheetController : Controller
    {
        private readonly ILogger<TimesheetController> _logger;
        private readonly TimesheetContext _Tcontext;
        private readonly LockTableContext _Lcontext;


        public TimesheetController(ILogger<TimesheetController> logger, TimesheetContext Tcontext,
            LockTableContext Lcontext)
        {
            _Tcontext = Tcontext;
            _Lcontext = Lcontext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Process(int id, string Status, string RejectionReason)
        {
            var timesheet = await _Tcontext.Timesheet
                .FirstOrDefaultAsync(m => m.TimesheetID == id);
            if (timesheet == null)
                return NotFound();
            var lockRow= await _Lcontext.LockTableRow
                .FirstOrDefaultAsync(m => m.TimesheetID == id);
            //Debug.WriteLine(lockRow.User);
            if (lockRow == null || !lockRow.User.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase))
                return View("NoPermission");
            timesheet.Status = Status;
            timesheet.RejectionReason = RejectionReason;
            timesheet.AdminActivity = Status + " by " + lockRow.User + " on " + DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _Tcontext.Update(timesheet);
                    await _Tcontext.SaveChangesAsync();
                    _Lcontext.Remove(lockRow);
                    await _Lcontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!_Tcontext.Timesheet.Any(e => e.TimesheetID == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }
    }
}