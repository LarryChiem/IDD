using System.Linq;
using System.Threading.Tasks;
using AdminUI.Models;
using Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminUI.Controllers
{
    [Authorize(Roles="Administrator")]
    public class LockTableController : Controller
    {
        private readonly SubmissionContext _context;

        public LockTableController(SubmissionContext context)
        {
            _context = context;
        }
        /*
         * Index is the main LockTable view
         * Parameters: The SortOrder and the page/perPage for pagination
         * Returns the LockTable Index View 
         */
        public IActionResult Index(string sortOrder="id", int page = 1, int perPage = 20)
        {
            var model = new LockTableModel();
            var submissions = _context.Submissions.Include(s=>s.LockInfo).Where(s => s.LockInfo != null);

            //big ol' switch statement determines how to sort the data in the table
            submissions = sortOrder switch
            {
                "id" => submissions.OrderBy(t => t.Id),
                "id_desc" => submissions.OrderByDescending(t => t.Id),
                "pname" => submissions.OrderBy(t => t.ProviderName),
                "pname_desc" => submissions.OrderByDescending(t => t.ProviderName),
                "prime" => submissions.OrderBy(t => t.ClientPrime),
                "prime_desc" => submissions.OrderByDescending(t => t.ClientPrime),
                "cname" => submissions.OrderBy(t => t.ClientName),
                "cname_desc" => submissions.OrderByDescending(t => t.ClientName),
                "date" => submissions.OrderBy(t => t.Submitted),
                "date_desc" => submissions.OrderByDescending(t => t.Submitted),
                "ProviderId" => submissions.OrderBy(t => t.ProviderId),
                "ProviderId_desc" => submissions.OrderByDescending(t => t.ProviderId),
                "lockedby" => submissions.OrderBy(t => t.LockInfo.User),
                "lockedby_desc" => submissions.OrderByDescending(t => t.LockInfo.User),
                "lastactivity" => submissions.OrderBy(t => t.LockInfo.LastActivity),
                "lastactivity_desc" => submissions.OrderByDescending(t => t.LockInfo.LastActivity),
                _ => submissions.OrderBy(t => t.Id),
            };
            model.SortOrder = sortOrder;
            model.TotalSubmissions = submissions.Count();

            model.TotalPages = submissions.Count() / perPage + (submissions.Count() % perPage == 0 ? 0 : 1);
            submissions = submissions.Skip((page - 1) * perPage).Take(perPage);
            model.PerPage = perPage;
            model.Page = page;

            model.Submissions = submissions.ToList();

            return View(model);
        }
        /*
         * Releases a lock
         * Parameters: The ID of the submission
         * Returns the LockTable Index
         */
        [HttpPost]
        public async Task<IActionResult> ReleaseLock(int id, string sortOrder)
        {
            var submission = _context.Submissions.Find(id);
            if (submission == null)
                return NotFound();
            _context.Entry(submission).Reference(t => t.LockInfo).Load();
            submission.LockInfo = null;
            _context.Update(submission);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "LockTable", new {sortOrder});
        }

        /*
         * Releases every lock in the database
         * Parameters: None
         * Returns the LockTable Index
         */
        public async Task<IActionResult> ReleaseAllLocks()
        {

            var submissions = _context.Submissions.Where(s => s.LockInfo != null).Include(s => s.LockInfo);
            foreach (var s in submissions)
            {
                s.LockInfo = null;
                _context.Update(s);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "LockTable");
        }

    }
}