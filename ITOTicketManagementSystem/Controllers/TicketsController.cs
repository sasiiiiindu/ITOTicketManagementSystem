using ITOTicketManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ITOTicketManagementSystem.Controllers
{
    [Authorize] // This ensures only logged-in users can access this controller
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TicketsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            // Get the ID of the currently logged-in user
            var userId = _userManager.GetUserId(User);

            // Retrieve only the tickets owned by that user
            var userTickets = await _context.Tickets
                                            .Where(t => t.OwnerId == userId)
                                            .OrderByDescending(t => t.CreatedDate)
                                            .ToListAsync();

            return View(userTickets);
        }

        [Authorize(Roles = "Help Desk Team, Admin")]
        public async Task<IActionResult> HelpDeskView()
        {
            var newTickets = await _context.Tickets
                                        .Include(t => t.Owner)
                                        .Where(t => t.CurrentAssignee == Models.AssigneeType.HelpDesk)
                                        .OrderBy(t => t.CreatedDate)
                                        .ToListAsync();

            return View(newTickets);
        }

        [Authorize(Roles = "Engineering Team, Admin")]
        public async Task<IActionResult> EngineeringView()
        {
            var assignedTickets = await _context.Tickets
                                            .Include(t => t.Owner)
                                            .Where(t => t.CurrentAssignee == Models.AssigneeType.Engineering)
                                            .OrderBy(t => t.CreatedDate)
                                            .ToListAsync();

            return View(assignedTickets);
        }

        [Authorize(Roles = "Help Desk Team, Admin")]
        public async Task<IActionResult> ExportToCsv()
        {
            // 1. Fetch the data to be exported (same query as HelpDeskView)
            var tickets = await _context.Tickets
                                    .Include(t => t.Owner)
                                    .Where(t => t.CurrentAssignee == Models.AssigneeType.HelpDesk)
                                    .OrderBy(t => t.CreatedDate)
                                    .ToListAsync();

            // 2. Build the CSV content using StringBuilder for efficiency
            var builder = new StringBuilder();
            builder.AppendLine("Id,Title,Status,CreatedDate,OwnerEmail"); // CSV Header

            // 3. Add a row for each ticket
            foreach (var ticket in tickets)
            {
                var ownerEmail = ticket.Owner?.Email ?? "N/A";
                builder.AppendLine($"{ticket.Id},\"{ticket.Title}\",{ticket.Status},{ticket.CreatedDate:yyyy-MM-dd HH:mm:ss},\"{ownerEmail}\"");
            }

            // 4. Return the CSV as a downloadable file
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"helpdesk-tickets-{DateTime.UtcNow:yyyy-MM-dd}.csv");
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.ViewModels.CreateTicketViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string? uniqueFileName = null; // Can be null if no file is uploaded

                // --- 1. Handle the file upload ---
                if (viewModel.Attachment != null)
                {
                    // The folder to store the attachments in
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "attachments");

                    // Create the folder if it doesn't exist
                    Directory.CreateDirectory(uploadsFolder);

                    // Create a unique file name to avoid conflicts
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Attachment.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Attachment.CopyToAsync(fileStream);
                    }
                }

                // --- 2. Create the Ticket object ---
                var ticket = new Models.Ticket
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    AttachmentPath = uniqueFileName, // Save only the file name
                    Status = Models.TicketStatus.New,
                    CreatedDate = DateTime.UtcNow,
                    OwnerId = _userManager.GetUserId(User),
                    CurrentAssignee = Models.AssigneeType.HelpDesk
                };

                // --- 3. Save to Database ---
                _context.Add(ticket);
                await _context.SaveChangesAsync();

                // --- 4. Redirect the user ---
                // We'll redirect to a "My Tickets" page later. For now, we redirect to Index.
                return RedirectToAction(nameof(Index));
            }

            // If the model is not valid, return the view with the entered data
            return View(viewModel);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the ticket by its ID, and include the Owner's details
            var ticket = await _context.Tickets
                .Include(t => t.Owner)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author) // For each comment, include its author
                .Include(t => t.History)
                    .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int ticketId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                // Optional: Add error handling if the comment is empty
                return RedirectToAction("Details", new { id = ticketId });
            }

            var newComment = new Models.Comment
            {
                Content = content,
                TicketId = ticketId,
                AuthorId = _userManager.GetUserId(User),
                CreatedDate = DateTime.UtcNow
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = ticketId });
        }

        [HttpPost]
        [Authorize(Roles = "Help Desk Team, Admin, Engineering Team")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int ticketId, Models.TicketStatus status)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            // --- 1. Create a History Record ---
            var historyRecord = new Models.TicketHistory
            {
                TicketId = ticket.Id,
                PropertyChanged = "Status",
                OldValue = ticket.Status.ToString(),
                NewValue = status.ToString(),
                ChangedDate = DateTime.UtcNow,
                UserId = _userManager.GetUserId(User)
            };
            _context.History.Add(historyRecord);

            // --- 2. Update the Ticket Status ---
            ticket.Status = status;
            _context.Update(ticket);

            // --- 3. Save Changes ---
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = ticketId });
        }

        [HttpPost]
        [Authorize(Roles = "Help Desk Team, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToEngineering(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            // --- 1. Create a History Record for the assignment change ---
            var historyRecord = new Models.TicketHistory
            {
                TicketId = ticket.Id,
                PropertyChanged = "Assignee",
                OldValue = ticket.CurrentAssignee.ToString(),
                NewValue = Models.AssigneeType.Engineering.ToString(),
                ChangedDate = DateTime.UtcNow,
                UserId = _userManager.GetUserId(User)
            };
            _context.History.Add(historyRecord);

            // --- 2. Update the Ticket's Assignee ---
            ticket.CurrentAssignee = Models.AssigneeType.Engineering;

            // Optional: Also change status to InProgress when assigning
            ticket.Status = Models.TicketStatus.InProgress;

            _context.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = ticketId });
        }

    }
}