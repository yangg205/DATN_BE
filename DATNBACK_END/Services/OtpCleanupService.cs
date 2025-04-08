// using DATNBACK_END.AppDataContexts;
// namespace DATNBACK_END.Services;
// public class OtpCleanupService : BackgroundService
// {
//     private readonly IServiceProvider _serviceProvider;
//     public OtpCleanupService(IServiceProvider serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             using (var scope = _serviceProvider.CreateScope())
//             {
//                 var context = scope.ServiceProvider.GetRequiredService<AppDataContexts.AppDataContext>();
//                 var expiredPending = context.PendingPlayer
//                     .Where(p => p.CreatedAt < DateTime.Now.AddMinutes(-5));

//                 context.PendingPlayer.RemoveRange(expiredPending);
//                 await context.SaveChangesAsync();
//             }
//             await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Kiểm tra mỗi phút
//         }
//     }
// }