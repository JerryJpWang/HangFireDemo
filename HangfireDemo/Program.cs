using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;

namespace HangfireDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Configure Hangfire to use SQL Server storage
            string connectionString = "Data Source=.;Initial Catalog=HangfireDemoDb;User ID=sa;Password=P@ss.123;Trusted_Connection=True;Encrypt=False;";
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(1) // Adjust polling interval
                });

            using (new BackgroundJobServer())
            {
                string jobId = "console-log-job";
                int currentSeconds = 10; // Default frequency
                Console.WriteLine("Application started, default execution every 10 seconds.");
                Console.WriteLine("Enter seconds to adjust execution frequency (e.g., 30 for every 30 seconds), enter 'exit' to quit.");

                var bojieList = new List<int>() { 5566, 7788, 7096, 9487 };

                while (true)
                {
                    string input = Console.ReadLine();
                    if (input?.ToLower() == "exit")
                        break;

                    if (int.TryParse(input, out int seconds) && seconds >= 5)
                    {
                        foreach (var bj in bojieList)
                        {
                            ScheduleJob($"{jobId}_{bj}", seconds);
                        }
                        currentSeconds = seconds;
                        Console.WriteLine($"Execution frequency updated to every {currentSeconds} seconds.");
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid number of seconds (minimum 5).");
                    }
                }
            }
        }

        private static void ScheduleJob(string jobId, int seconds)
        {
            try
            {
                RecurringJob.AddOrUpdate(
                    jobId,
                    () => ConsoleLogJobAsync(),
                    $"*/{seconds} * * * * *");
                Console.WriteLine($"Job {jobId} scheduled successfully, running every {seconds} seconds.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling job: {ex.Message}");
            }
        }

        public static void ConsoleLogJob()
        {
            Console.WriteLine($"Task execution time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        public static async Task ConsoleLogJobAsync()
        {
            Console.WriteLine($"Async task execution time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            await Task.CompletedTask; // Indicates async task completion
        }
    }
}