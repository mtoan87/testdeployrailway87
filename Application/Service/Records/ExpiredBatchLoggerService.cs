//using Application.Interfaces;
//using Domain.Model;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Service.Records
//{
//    public class ExpiredBatchLoggerService : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly ILogger<ExpiredBatchLoggerService> _logger;
//        public ExpiredBatchLoggerService(IServiceProvider serviceProvider, ILogger<ExpiredBatchLoggerService> logger)
//        {
//            _serviceProvider = serviceProvider;
//            _logger = logger;
//        }
//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                using var scope = _serviceProvider.CreateScope();
//                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
//                var now = DateTime.Now;

//                // 1. Lấy tất cả các batch để cập nhật DaysUntilExpiration và xử lý log
//                var allBatches = await unitOfWork.BatchDetailRepo.GetAllAsync();

//                foreach (var batch in allBatches)
//                {
//                    if (batch.ExpiredDate.HasValue)
//                    {
//                        // Cập nhật DaysUntilExpiration (có thể là số âm nếu đã hết hạn)
//                        batch.DaysUntilExpiration = (batch.ExpiredDate.Value.Date - now.Date).Days;
//                    }
//                }

//                // 2. Lọc các batch đã hết hạn và chưa log
//                var expiredBatches = allBatches
//                    .Where(b => b.ExpiredDate <= now && b.IsExpiredLogged == false)
//                    .ToList();

//                foreach (var batch in expiredBatches)
//                {
//                    var log = new Log
//                    {
//                        ProductId = batch.ProductId,
//                        Quantity = batch.RemainingQuantity ?? 0,
//                        Type = "Expired",
//                        CreateDate = now,
//                        Note = $"Gói nhập hết hạn! Số còn lại: {batch.RemainingQuantity ?? 0}/{batch.Quantity}",
//                        ExpiredDate = batch.ExpiredDate,
//                        BatchDetailId = batch.Id,
//                        IsDeleted = false
//                    };

//                    await unitOfWork.LogRepo.AddAsync(log);
//                    batch.IsExpiredLogged = true;
//                }

//                // 3. Lưu tất cả thay đổi một lần
//                await unitOfWork.SaveChangeAsync();

//                _logger.LogInformation("Cập nhật DaysUntilExpiration và log batch hết hạn lúc {Time}", now);

//                // 4. Đợi 1 tiếng trước khi chạy lại
//                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
//            }
//        }
//    }
//}
