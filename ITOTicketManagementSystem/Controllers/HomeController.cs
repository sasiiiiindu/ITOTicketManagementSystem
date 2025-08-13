using ITOTicketManagementSystem.Data;
using ITOTicketManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ITOTicketManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var viewModel = new Models.ViewModels.DashboardViewModel
            {
                NewTicketsCount = await _context.Tickets.CountAsync(t => t.Status == Models.TicketStatus.New),
                InProgressTicketsCount = await _context.Tickets.CountAsync(t => t.Status == Models.TicketStatus.InProgress),
                ResolvedTicketsCount = await _context.Tickets.CountAsync(t => t.Status == Models.TicketStatus.Resolved),
                ClosedTicketsCount = await _context.Tickets.CountAsync(t => t.Status == Models.TicketStatus.Closed),
                TotalTicketsCount = await _context.Tickets.CountAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}