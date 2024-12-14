using FetchSocialMediaPosts.Context;
using FetchSocialMediaPosts.DTO;
using FetchSocialMediaPosts.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace FetchSocialMediaPosts.Platforms.Facebook
{
    public class FacebookPosts : ISocialMediaService
    {
        private readonly int _maxPosts;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _accessToken;

        public string SocialMediaName => "Facebook";

        public FacebookPosts(int maxPosts, IHttpClientFactory httpClientFactory)
        {
            _maxPosts = maxPosts;
            _httpClient = httpClientFactory.CreateClient();
            _accessToken = "Token";
            _baseUrl = $"https://graph.facebook.com/v21.0/me/posts?fields=id,from,created_time,message,permalink_url&access_token={_accessToken}";
        }

        public async Task<List<PostDto>> GetPostsAsync()
        {
            var postsResponse = new List<PostDto>();

            try
            {
                Console.WriteLine("===================================================================================================\n\n");
                Console.WriteLine($"Fetching posts from Facebook\n\n");
                Console.WriteLine("===================================================================================================\n\n");

                var response = await _httpClient.GetAsync(_baseUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching Facebook posts: {response.ReasonPhrase}");
                    return new List<PostDto>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                var posts = json["data"]?.ToArray();

                if (posts == null || posts.Length == 0)
                {
                    Console.WriteLine("No posts found.");
                    return postsResponse;
                }

                var randomPosts = posts
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(_maxPosts);

                foreach (var postData in randomPosts)
                {
                    var id = postData["id"]?.ToString();
                    var message = postData["message"]?.ToString() ?? "No Content";
                    var createdTime = postData["created_time"].ToString();
                    var authorName = postData["from"]?["name"]?.ToString() ?? "Default";
                    var postLink = postData["permalink_url"]?.ToString() ?? "No link";

                    var post = new PostDto
                    {
                        Id = id,
                        Author = authorName,
                        Content = message,
                        FetchedFrom = SocialMediaName,
                        FetchedAt = DateTime.Now,
                        CreatedAt = DateTime.Parse(createdTime),
                        PostLink = postLink
                    };

                    postsResponse.Add(post);
                }

                Console.WriteLine("===================================================================================================\n\n");
                Console.WriteLine($"Completed fetching posts from Facebook\n\n");
                Console.WriteLine("===================================================================================================\n\n");

                return postsResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Facebook posts: {ex.Message}");
                return new List<PostDto>();
            }
        }
    }
}
