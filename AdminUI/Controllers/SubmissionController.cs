using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data;
using System.IO;
using Common.Models;
using Microsoft.AspNetCore.Authorization;

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

        /*
         * Index returns a detailed Submission View
         * Parameters: the ID of the Submission
         * Returns: Index.cshtml view
         */
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

            return View(submission);
        }

        /*
         * GenPDF() creates a PDF of a submission and sends it to the web client for download
         * Parameters: the ID of the submission
         * Returns the PDF summary of the submission
         */
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

            //ClientName_Prime_ProviderID_ProviderName_yyyyMMdd_FormNumber
            var fileDownloadName = submission.ClientName + "_" + submission.ClientPrime + "_" + submission.ProviderId + "_" +
                                   submission.ProviderName + "_" + submission.Submitted.ToString("yyyyMMdd") + "_" + submission.FormType.Split(" ")[0] + ".pdf";

            fileDownloadName = fileDownloadName.Replace(" ", string.Empty);

            return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, fileDownloadName);
        }

        /*
         * Process() sets the status of the submission to either approved or rejected
         * Parameters: The ID of the submission, the status, the rejection reason, the sort order of the able, the filters, and a string indicating whether or not this was process by the modal
         * Returns either the next available submission or the Home Index, depending on the value of 'modal'
         */
        [HttpPost]
        public async Task<IActionResult> Process(int id, string status, string rejectionReason, string sortOrder="", string dateFrom="", string dateTo="", string pName="", string cName="", 
            string prime="", string formType="", int page=0, string providerId="", string modal="false")
        {
            status = status.Equals("Approve") ? "Approved" : "Rejected";

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
            
            submission.LoadEntries(_context);
            submission.ChangeAllEntriesStatus(status);

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

            if (modal.Equals("true"))
                return RedirectToAction("Index", "Home", new { SortOrder = sortOrder, DateTo = dateTo, DateFrom = dateFrom, PName = pName, CName = cName, Prime = prime, FormType = formType, Page = page, ProviderId = providerId});

            var next = _context.Submissions.FirstOrDefault(s => (s.Status == null || s.Status == "Pending") && (s.LockInfo == null || s.LockInfo.User == User.Identity.Name));
            return next != null ? RedirectToAction("Index", new {Id = next.Id}) : RedirectToAction("Index", "Home");
        }

        /*
         * ProcessLine() processes individual rows on the submissions
         * Parameters: The submission ID, the entry ID, and the status to be set
         * Returns the Index of the submission ID
         */
        [HttpPost]
        public IActionResult ProcessLine(int submissionId, int entryId, string status)
        {
            status = status.Equals("Approve") ? "Approved" : "Rejected";

            var submission = _context.Submissions.Find(submissionId);
            submission.LoadEntries(_context);
            submission.ChangeEntryStatus(entryId, status);
            _context.Update(submission);
            _context.SaveChanges();
            return RedirectToAction("Index", new {Id = submissionId});
        }

    }
}
