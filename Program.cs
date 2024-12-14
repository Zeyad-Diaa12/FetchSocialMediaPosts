using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FetchSocialMediaPosts.Context;
using FetchSocialMediaPosts.DTO;
using FetchSocialMediaPosts.Interface;
using FetchSocialMediaPosts.Platforms.Facebook;
using FetchSocialMediaPosts.Platforms.Instagram;
using FetchSocialMediaPosts.Platforms.Twitter;
using FetchSocialMediaPosts.Service;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        //var services = new ServiceCollection();

        //services.AddDbContext<PostsDbContext>(options =>
        //    options.UseSqlite("Data Source=socialmedia.db"),
        //    ServiceLifetime.Transient);

        ////services.AddTransient<ISocialMediaService, TwitterPosts>(provider =>
        ////    new TwitterPosts(10, provider.GetRequiredService<PostsDbContext>()));

        ////services.AddTransient<ISocialMediaService, InstagramPosts>(provider =>
        ////    new InstagramPosts(10, provider.GetRequiredService<PostsDbContext>()));
        
        ////services.AddTransient<ISocialMediaService, FacebookPosts>(provider =>
        ////    new FacebookPosts(10, provider.GetRequiredService<PostsDbContext>()));

        //var serviceProvider = services.BuildServiceProvider();

        //var fetcher = new SocialMediaFetcher(refreshIntervalSeconds: 5);

        //var socialMediaServices = serviceProvider.GetServices<ISocialMediaService>();

        //foreach (var service in socialMediaServices)
        //{
        //    fetcher.AddService(service);
        //}

        //fetcher.PostsUpdated += (sender, posts) =>
        //{
        //    Console.WriteLine("\n--- Updated Posts ---");
        //    foreach (var post in posts)
        //    {
        //        Console.WriteLine($"[{post.FetchedFrom}] {post.Author}: {post.Content} (Fetched: {post.FetchedAt})");
        //    }
        //};

        //await fetcher.StartAsync();
        //Console.WriteLine("Social Media Fetcher running. Press any key to exit.");
        //Console.ReadKey();
        //fetcher.Stop();
    }
}