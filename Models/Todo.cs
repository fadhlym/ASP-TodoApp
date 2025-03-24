using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Todo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please enter a due date")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public string? CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; } = null!;

        [Required(ErrorMessage = "Please select a status")]
        public string StatusId { get; set; } = null!;

        [ValidateNever]
        public Status Status { get; set; } = null!;

        public bool Overdue => StatusId == "open" && DueDate < DateTime.Today;

    }
}
