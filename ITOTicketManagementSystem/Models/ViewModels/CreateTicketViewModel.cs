using System.ComponentModel.DataAnnotations;

namespace ITOTicketManagementSystem.Models.ViewModels
{
    public class CreateTicketViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // This property will hold the uploaded file
        public IFormFile? Attachment { get; set; }
    }
}