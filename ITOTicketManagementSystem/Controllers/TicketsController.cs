using ITOTicketManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                                        .Where(t => t.Status == Models.TicketStatus.New)
                                        .OrderBy(t => t.CreatedDate)
                                        .ToListAsync();

            return View(newTickets);
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
                    OwnerId = _userManager.GetUserId(User)
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

    }
}