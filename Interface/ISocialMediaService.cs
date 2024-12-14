using FetchSocialMediaPosts.DTO;
using System.Collections.Concurrent;

namespace FetchSocialMediaPosts.Interface
{
    public interface ISocialMediaService
    {
        string SocialMediaName { get; }
        Task<List<PostDto>> GetPostsAsync();
    }
}
