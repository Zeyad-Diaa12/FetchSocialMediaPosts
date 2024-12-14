namespace FetchSocialMediaPosts.DTO
{
    public class PostDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string FetchedFrom { get; set; }
        public string PostLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime FetchedAt { get; set; }
    }
}
