using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdminUI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data;

namespace AdminUI.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly SubmissionContext _scontext;


        public SubmissionController(ILogger<SubmissionController> logger, SubmissionContext scontext)
        {
            _logger = logger;
            _scontext = scontext;
        }

        [HttpPost]
        public async Task<IActionResult> Process(int id, string Status, string RejectionReason)
        {
            var submission = _scontext.Submissions.Find(id);

            if (submission == null)
                return NotFound();
            
            _scontext.Entry(submission).Reference(t => t.LockInfo).Load();
            
            if (submission.LockInfo == null || !submission.LockInfo.User.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase))
                return View("NoPermission");
            
            submission.Status = Status;
            submission.RejectionReason = RejectionReason;
            submission.UserActivity = Status + " by " + submission.LockInfo.User + " on " + DateTime.Now;
            submission.LockInfo = null;
            
            if (ModelState.IsValid)
            {
                try
                {
                    _scontext.Update(submission);
                    await _scontext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!_scontext.Submissions.Any(e => e.Id == id))
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