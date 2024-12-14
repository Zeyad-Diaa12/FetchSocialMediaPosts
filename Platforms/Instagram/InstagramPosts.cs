using FetchSocialMediaPosts.Context;
using FetchSocialMediaPosts.DTO;
using FetchSocialMediaPosts.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FetchSocialMediaPosts.Platforms.Instagram
{
    public class InstagramPosts : ISocialMediaService
    {
        private readonly int _maxPosts;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _accessToken;

        public string SocialMediaName => "Instagram";

        public InstagramPosts(int maxPosts, IHttpClientFactory httpClientFactory)
        {
            _maxPosts = maxPosts;
            _httpClient = httpClientFactory.CreateClient();
            _accessToken = "Token";
            _baseUrl = $"https://graph.instagram.com/9077243345697411/media?fields=id,media_url,permalink,username,timestamp,caption&access_token={_accessToken}";
        }

        public async Task<List<PostDto>> GetPostsAsync()
        {
            var postsResponse = new List<PostDto>();

            try
            {
                Console.WriteLine("===================================================================================================\n\n");
                Console.WriteLine($"Fetching posts from Instagram\n\n");
                Console.WriteLine("===================================================================================================\n\n");


                var response = await _httpClient.GetAsync(_baseUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching Instagram posts: {response.ReasonPhrase}");
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
                    var message = postData["caption"]?.ToString() ?? "No Content";
                    var createdTime = postData["timestamp"]?.ToString();
                    var authorName = postData["username"]?.ToString() ?? "Default";
                    var postLink = postData["permalink"]?.ToString() ?? "No link";

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
                Console.WriteLine($"Completed fetching posts from Instagram\n\n");
                Console.WriteLine("===================================================================================================\n\n");

                return postsResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Instagram posts: {ex.Message}");
                return new List<PostDto>();
            }
        }
    }
}
