using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ITOTicketManagementSystem.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        // Foreign Key for the Ticket
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        // Foreign Key for the User who authored the comment
        public string AuthorId { get; set; }
        public virtual IdentityUser Author { get; set; }
    }
}