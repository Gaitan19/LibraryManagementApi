using System.ComponentModel.DataAnnotations;

namespace libraryManegement.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        public List<Book> Books { get; set; } = new();

    }
}
