namespace BetaCycleAPI.BLogic
{
    public class RetrainingService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<RetrainingService> _logger;
        private Timer? _timer = null;

        public RetrainingService(ILogger<RetrainingService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            ScheduleRetraining();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void Retrain()
        {
            RecommendProduct.Train("BLogic/RecommendProduct/RecommendProduct.mlnet");
        }

        private void ScheduleRetraining()
        {
            _timer = new Timer(DoRetraining, null, TimeSpan.Zero, TimeSpan.FromHours(8));
        }

        private void DoRetraining(object? state)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Retraining review recommendations...");
            Console.ForegroundColor = ConsoleColor.White;
            Retrain();
        }
    }
}
