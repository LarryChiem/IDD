using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System.IO;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using SQLitePCL;

namespace AdminUI.Controllers
{
    [Authorize]
    public class SubmissionController : Controller
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly SubmissionContext _context;


        public SubmissionController(ILogger<SubmissionController> logger, SubmissionContext context)
        {
            _logger = logger;
            _context = context;
        }

        //should return a timesheet View
        public IActionResult Index(int id)
        {
            var submission =  _context.Submissions.Find(id);
            _context.Entry(submission).Reference(t => t.LockInfo).Load();

            if (submission.LockInfo == null)
            {
                submission.LockInfo = new Lock
                {
                    LastActivity = DateTime.Now,
                    User = User.Identity.Name
                };
                _context.Update(submission);
                _context.SaveChanges();
            }

            submission.LoadEntries(_context);
            return View(submission.GetType().Name, submission);
        }

        public async Task<IActionResult> GenPDF(int id)
        {
            //find the timesheet code
            var submission = await _context.Submissions
                .FirstOrDefaultAsync(m => m.Id == id);

            if (submission == null)
                return NotFound();

            //Load the timesheet's entries
            submission.LoadEntries(_context);

            var stream = new MemoryStream();
            
            // Save the document
            submission.ToPdf().Save(stream, true);

            var fileDownloadName = submission.ClientName + "_" + submission.ClientPrime + "_" + submission.ProviderId + "_" +
                                   submission.ProviderName + "_" + submission.Submitted + "_" + submission.FormType + ".pdf";

            return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, fileDownloadName);
        }

        [HttpPost]
        public async Task<IActionResult> Process(int id, string status, string rejectionReason)
        {
            var submission = _context.Submissions.Find(id);

            if (submission == null)
                return NotFound();
            
            _context.Entry(submission).Reference(t => t.LockInfo).Load();
            
            if (submission.LockInfo == null || !submission.LockInfo.User.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase))
                return View("NoPermission");
            
            submission.Status = status;
            submission.RejectionReason = rejectionReason;
            submission.UserActivity = status + " by " + submission.LockInfo.User + " on " + DateTime.Now;
            submission.LockInfo = null;
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(submission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!_context.Submissions.Any(e => e.Id == id))
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
