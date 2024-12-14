using FetchSocialMediaPosts.DTO;
using FetchSocialMediaPosts.Interface;
using System.Collections.Concurrent;

namespace FetchSocialMediaPosts.Service
{
    public class SocialMediaFetcher : IDisposable
    {
        private readonly List<ISocialMediaService> _socialMediaServices;
        private ConcurrentBag<PostDto> _fetchedPosts;
        private CancellationTokenSource _cancellationTokenSource;
        private System.Timers.Timer? _refreshTimer;

        public event EventHandler<List<PostDto>>? PostsUpdated;

        public SocialMediaFetcher()
        {
            _socialMediaServices = new List<ISocialMediaService>();
            _fetchedPosts = new ConcurrentBag<PostDto>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void AddService(ISocialMediaService service)
        {
            _socialMediaServices.Add(service);
        }

        public async Task StartAsync(int numberOfThreads, int refreshInterval = 30)
        {
            Reset();
            Console.WriteLine($"Starting fetch with {numberOfThreads} threads");
            await FetchPostsAsync(numberOfThreads);

            if (refreshInterval > 0)
            {
                StartRefreshTimer(numberOfThreads, refreshInterval);
            }
        }

        public async Task RefreshAsync(int numberOfThreads)
        {
            Console.WriteLine("Refreshing posts...");

            _fetchedPosts = new ConcurrentBag<PostDto>();

            await FetchPostsAsync(numberOfThreads);
        }

        private async Task FetchPostsAsync(int numberOfThreads)
        {
            if (_cancellationTokenSource?.IsCancellationRequested ?? true)
            {
                Console.WriteLine("Operation cancelled or token source disposed.");
                return;
            }

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = numberOfThreads,
                CancellationToken = _cancellationTokenSource.Token
            };

            await Parallel.ForEachAsync(_socialMediaServices, options, async (service, token) =>
            {
                await RunServiceAsync(service, token);
            });

            PostsUpdated?.Invoke(this, _fetchedPosts.ToList());
        }


        private async Task RunServiceAsync(ISocialMediaService service, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Fetch operation cancelled");
                    return;
                }

                Console.WriteLine("===================================================================================================\n");
                Console.WriteLine($"{service.SocialMediaName} is running on thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
                Console.WriteLine("===================================================================================================\n");

                var posts = await service.GetPostsAsync();

                foreach (var post in posts)
                {
                    _fetchedPosts.Add(post);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching posts from {service.SocialMediaName}: {ex.Message}");
            }
        }

        private void StartRefreshTimer(int numberOfThreads, int sec)
        {
            _refreshTimer = new System.Timers.Timer(sec * 1000);
            _refreshTimer.Elapsed += async (sender, e) =>
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await RefreshAsync(numberOfThreads);
                }
            };
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();
        }

        public void Reset()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _fetchedPosts = new ConcurrentBag<PostDto>();
            _refreshTimer = null;
        }

        public void Stop()
        {
            _refreshTimer?.Stop();
            _cancellationTokenSource.Cancel();
            Dispose();
        }


        public void Dispose()
        {
            _refreshTimer?.Dispose();
            _cancellationTokenSource?.Dispose();
        }
    }
}
