namespace libraryManegement.Dtos
{
    public class CreateBookDto
    {
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string ISBN { get; set; }

        public required int PublicationYear { get; set; }
    }

}
