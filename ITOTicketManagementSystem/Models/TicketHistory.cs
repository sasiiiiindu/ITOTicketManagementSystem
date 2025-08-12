using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ITOTicketManagementSystem.Models
{
    public class TicketHistory
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key for the Ticket
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        // What property was changed (e.g., "Status", "Assigned To")
        public string PropertyChanged { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ChangedDate { get; set; }

        // Foreign Key for the User who made the change
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}