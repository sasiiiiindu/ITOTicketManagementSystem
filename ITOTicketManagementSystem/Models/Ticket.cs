using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ITOTicketManagementSystem.Models
{
    // This defines the possible statuses a ticket can have.
    public enum TicketStatus
    {
        New,
        InProgress,
        Resolved,
        Closed
    }

    // This defines the possible teams a ticket can be assigned to
    public enum AssigneeType
    {
        HelpDesk,
        Engineering
    }

    public class Ticket
    {
        [Key] // This marks the Id property as the primary key
        public int Id { get; set; }

        [Required] // This makes the Title a required field
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // This uses the Enum we defined above for the status
        public TicketStatus Status { get; set; }

        // This will store the path to the uploaded file, if any
        public string? AttachmentPath { get; set; }

        public DateTime CreatedDate { get; set; }

        public Ticket()
        {
            // Set default values when a new ticket is created
            Status = TicketStatus.New;
            CreatedDate = DateTime.UtcNow;
        }
        public string? OwnerId { get; set; }
        public virtual IdentityUser? Owner { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public virtual ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();

        public AssigneeType CurrentAssignee { get; set; }
    }

}