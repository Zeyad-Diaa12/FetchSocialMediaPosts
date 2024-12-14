﻿namespace FetchSocialMediaPosts.DTO
{
    public class TwitterPost
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public required string Author { get; set; }
        public required string FetchedFrom { get; set; }
    }
}
