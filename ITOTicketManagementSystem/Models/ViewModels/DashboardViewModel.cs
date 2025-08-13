namespace ITOTicketManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int NewTicketsCount { get; set; }
        public int InProgressTicketsCount { get; set; }
        public int ResolvedTicketsCount { get; set; }
        public int ClosedTicketsCount { get; set; }
        public int TotalTicketsCount { get; set; }
    }
}