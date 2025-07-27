namespace BaiTap1.Models
{
    public class BookModel
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public Guid AuthorId { get; set; }
    }
}
