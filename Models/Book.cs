using System.ComponentModel.DataAnnotations;

namespace libraryManegement.Models
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public int PublicationYear { get; set; }

        [Required]
        public string ISBN { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public Guid? UserId { get; set; }
        //public User User { get; set; } = null!;
    }
}
