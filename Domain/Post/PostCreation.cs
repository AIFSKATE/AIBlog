namespace Domain.Post
{
    public class PostCreation
    {
        public required string Title { get; set; }
        public required string Markdown { get; set; }
        public List<int> TagIDs { get; set; } = new List<int>();
        public int? CategoryId { get; set; }
    }
}
