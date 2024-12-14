using FetchSocialMediaPosts.Context;
using FetchSocialMediaPosts.DTO;
using FetchSocialMediaPosts.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Threading.Tasks;

namespace FetchSocialMediaPosts.Platforms.Twitter
{
    public class TwitterPosts : ISocialMediaService
    {
        private readonly int _maxPosts;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public string SocialMediaName => "Twitter";

        public TwitterPosts(int maxPosts, IHttpClientFactory httpClientFactory)
        {
            _maxPosts = maxPosts;
            _httpClient = httpClientFactory.CreateClient();
            _baseUrl = $"https://api.twitter.com/2/tweets/search/recent?query=RealMadrid&tweet.fields=article,attachments,created_at,id,text&media.fields=url&user.fields=name";
        }

        public async Task<List<PostDto>> GetPostsAsync()
        {
            var postsResponse = new List<PostDto>();
            var _bearerToken = "Token";

            try
            {
                Console.WriteLine("===================================================================================================\n\n");
                Console.WriteLine($"Fetching posts from Twitter thread ID : {Thread.CurrentThread.ManagedThreadId}\n\n");
                Console.WriteLine("===================================================================================================\n\n");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

                var response = await _httpClient.GetAsync(_baseUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error fetching Twitter posts: {response.ReasonPhrase}");
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
                    var message = postData["text"]?.ToString() ?? "No Content";
                    var createdTime = postData["created_at"].ToString();
                    var authorName = "Default";
                    var postLink = "No link";

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
                Console.WriteLine($"Completed fetching posts from Twitter\n\n");
                Console.WriteLine("===================================================================================================\n\n");

                return postsResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Twitter posts: {ex.Message}");
                return new List<PostDto>();
            }
        }
    }
}
