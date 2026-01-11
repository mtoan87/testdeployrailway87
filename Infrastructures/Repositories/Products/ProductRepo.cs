using Application.Commons;
using Application.DTO.BatchDetails;
using Application.DTO.Categories;
using Application.DTO.Dashboards;
using Application.DTO.Images;
using Application.DTO.Products;
using Application.DTO.Records;
using Application.Interfaces;
using Application.IRepositories.Products;
using Domain.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.Products
{
    public class ProductRepo : GenericRepository<Product>, IProductRepo
    {
        private readonly HypeCatDbContext _context;
        public ProductRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _context = context;
        }

        public async Task<List<TopProductDTO>> GetTop5BestSellingProductsAsync()
        {
            var result = await _context.OrderDetails
                .Where(od => od.ProductId != null)
                .GroupBy(od => od.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key.Value,
                    TotalQuantity = group.Sum(x => x.Quantity ?? 0)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(5)
                .Join(_context.Products.Include(p => p.Images),
                      g => g.ProductId,
                      p => p.Id,
                      (g, p) => new TopProductDTO
                      {
                          ProductId = p.Id,
                          ProductName = p.Name,
                          //Category = p.Category,
                          //SellingPrice = p.SellingPrice ?? 0,
                          TotalQuantitySold = g.TotalQuantity,
                          ImageUrls = p.Images
                              .Where(img => !img.IsDeleted)
                              .Select(img => img.UrlPath)
                              .ToList()
                      })
                .ToListAsync();

            return result;
        }
        public async Task<Pagination<Product>> GetPaginationAsync(ProductPaginationDTO paginationDTO)
        {
            var query = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Logs)
                .Include(p => p.BatchDetails)
                .Include(p => p.Category)

                .AsQueryable();

            // Apply IsDeleted filter
            if (paginationDTO.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == paginationDTO.IsDeleted);
            }

            // Apply search filters
            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(paginationDTO.SearchTerm)); 
                    //p.Category.Contains(paginationDTO.SearchTerm) ||
                    //p.SourceOfProducts.Contains(paginationDTO.SearchTerm));
            }

            if (!string.IsNullOrEmpty(paginationDTO.Category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name.Contains(paginationDTO.Category));
            }

            //if (paginationDTO.MinPrice.HasValue)
            //{
            //    query = query.Where(p => p.SellingPrice >= paginationDTO.MinPrice);
            //}

            //if (paginationDTO.MaxPrice.HasValue)
            //{
            //    query = query.Where(p => p.SellingPrice <= paginationDTO.MaxPrice);
            //}

            if (!string.IsNullOrEmpty(paginationDTO.Status))
            {
                query = query.Where(p => p.Status == paginationDTO.Status);
            }

            //if (!string.IsNullOrEmpty(paginationDTO.SourceOfProducts))
            //{
            //    query = query.Where(p => p.SourceOfProducts == paginationDTO.SourceOfProducts);
            //}

            // Apply sorting
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {
                    "name" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    //"price" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.SellingPrice) : query.OrderBy(p => p.SellingPrice),
                    //"stock" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
                    "createdate" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate),
                    "isdeleted" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.IsDeleted) : query.OrderBy(p => p.IsDeleted),
                    _ => paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate)
                };
            }
            else
            {
                query = paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
                .Take(paginationDTO.PageSize)
                .ToListAsync();

            return new Pagination<Product>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }
        public async Task<bool> CheckStockQuantityAsync(int? productId, int requiredQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new Exception($"Product with id {productId} does not exist.");
            }

            return product.Id >= requiredQuantity;
        }

        public async Task<List<Product>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Products
                                 .Where(p => ids.Contains(p.Id))
                                 .ToListAsync();
        }
       
        public async Task<byte[]> ExportInventoryReportAsync(InventoryExportRequest request)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var logs = await _context.Logs
                .Include(x => x.Product)
                .Where(x => x.CreateDate >= request.FromDate && x.CreateDate <= request.ToDate)
                .ToListAsync();

            var grouped = logs
                .Where(x => x.Product != null)
                .GroupBy(x => x.ProductId)
                .ToList();

            using var package = new ExcelPackage();
            var existingNames = new HashSet<string>();

            if (!grouped.Any())
            {
                var worksheet = package.Workbook.Worksheets.Add("EmptyReport");
                worksheet.Cells[1, 1].Value = "Không có dữ liệu cho khoảng thời gian đã chọn.";
            }
            else
            {
                foreach (var group in grouped)
                {
                    var product = group.First().Product!;
                    var baseSheetName = string.Concat(product.Name.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
                    if (string.IsNullOrWhiteSpace(baseSheetName))
                        baseSheetName = "Product";

                    var sheetName = baseSheetName;
                    int suffix = 1;

                    while (existingNames.Contains(sheetName) || package.Workbook.Worksheets.Any(ws => ws.Name == sheetName))
                    {
                        sheetName = $"{baseSheetName}_{suffix++}";
                    }

                    existingNames.Add(sheetName);
                    var worksheet = package.Workbook.Worksheets.Add(sheetName);

                    // Header
                    worksheet.Cells[1, 1].Value = "Ngày";
                    worksheet.Cells[1, 2].Value = "Loại giao dịch";
                    worksheet.Cells[1, 3].Value = "Số lượng";
                    worksheet.Cells[1, 4].Value = "Tồn kho sau giao dịch";

                    int row = 2;

                    // Tính tồn kho ban đầu trước khoảng thời gian (chỉ tính Import/Export)
                    var initialLogs = await _context.Logs
                        .Where(x => x.ProductId == product.Id && x.CreateDate < request.FromDate && x.Type != "UpdatePrice")
                        .ToListAsync();

                    int initialStock = initialLogs.Sum(x => x.Type == "Import" ? x.Quantity ?? 0 : -(x.Quantity ?? 0));
                    int runningStock = initialStock;

                    // Dữ liệu từng log (bỏ qua UpdatePrice)
                    foreach (var log in group
                        .Where(x => x.Type != "UpdatePrice")
                        .OrderBy(x => x.CreateDate))
                    {
                        int qty = log.Quantity ?? 0;
                        if (log.Type == "Import")
                            runningStock += qty;
                        else if (log.Type == "Export")
                            runningStock -= qty;

                        worksheet.Cells[row, 1].Value = log.CreateDate?.ToString("dd/MM/yyyy HH:mm");
                        worksheet.Cells[row, 2].Value = log.Type == "Import" ? "Nhập Hàng" : "Xuất Hàng";
                        worksheet.Cells[row, 3].Value = qty;
                        worksheet.Cells[row, 4].Value = runningStock;

                        row++;
                    }

                    // Tổng số lượng (bỏ qua UpdatePrice)
                    worksheet.Cells[row + 1, 2].Value = "Tổng giao dịch:";
                    worksheet.Cells[row + 1, 3].Value = group
                        .Where(x => x.Type != "UpdatePrice")
                        .Sum(x => x.Quantity ?? 0);

                    // Tồn kho cuối
                    worksheet.Cells[row + 2, 2].Value = "Tồn cuối:";
                    worksheet.Cells[row + 2, 3].Value = runningStock;

                    worksheet.Cells.AutoFitColumns();
                }
            }

            return await package.GetAsByteArrayAsync();
        }

        public async Task<List<BatchDetailDTO>> GetProductBatchDetailByProductId(int productId)
        {
            var product = await _context.Products
                .Include(p => p.BatchDetails)
                .ThenInclude(bd => bd.Batch)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new Exception($"Product with id {productId} not found.");
            }

            var batchDetails = product.BatchDetails
                .Select(bd => new BatchDetailDTO
                {
                    //BatchDetailId = bd.Id,
                    BatchId = bd.BatchId,
                    SellingPrice = bd.SellingPrice,
                    ImportCosts = bd.ImportCosts,
                    //ExpiredDate = bd.ExpiredDate,
                    Quantity = bd.Quantity,
                    RemainingQuantity = bd.RemainingQuantity,
                    // nếu muốn lấy thêm thông tin từ bảng Batch
                })
                .ToList();

            return batchDetails;
        }
        public async Task<Pagination<ProductUserListDTO>> GetUserProductPaginationAsync(ProductPaginationDTO paginationDTO)
        {
            var query = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.BatchDetails)                 
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(paginationDTO.SearchTerm));
            }
            if (!string.IsNullOrEmpty(paginationDTO.Category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name.Contains(paginationDTO.Category));
            }
            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(paginationDTO.Status))
            {
                query = query.Where(p => p.Status == paginationDTO.Status);
            }

            // Sắp xếp
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {
                    "name" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "createdate" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate),
                    _ => query.OrderByDescending(p => p.CreateDate)
                };
            }

            // Tổng số lượng
            var totalCount = await query.CountAsync();

            // Phân trang
            var products = await query
                .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
                .Take(paginationDTO.PageSize)
                .ToListAsync();

            // Lấy thông tin giá mới nhất từ BatchDetails
            var productDTOs = products.Select(p =>
            {
                var latestBatch = p.BatchDetails
                    .Where(b => b.RemainingQuantity > 0) // Lọc các batch không bị xóa
                    .OrderBy(b => b.CreateDate)
                    .FirstOrDefault();

                return new ProductUserListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status,
                    Language = p.Language,
                    Cover = p.Cover,
                    BoxId = p.BoxId,
                    //CategoryId = p.CategoryId,
                    //CreateDate = p.CreateDate,
                    //IsDeleted = p.IsDeleted,
                    Category = p.Category?.Adapt<CategoryProductDTO>(),
                    Images = p.Images?.Select(i => i.Adapt<ImageDTO>()).ToList() ?? new List<ImageDTO>(),
                    BatchDetails = new List<BatchDetailUserProductDTO>
            {
                latestBatch?.Adapt<BatchDetailUserProductDTO>()!
            },
                    //Logs = new List<ProductUserListDTO>() // có thể bỏ qua nếu không cần
                };
            }).ToList();

            return new Pagination<ProductUserListDTO>
            {
                Items = productDTOs,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }

        public async Task<ProductUserListDTO> GetUserProductByIdWithFiltersAsync(int? productId, ProductPaginationDTO paginationDTO)
        {
            var query = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.BatchDetails)
                .Where(p => !p.IsDeleted && p.Id == productId) // lọc theo productId
                .AsQueryable();

            // Lọc theo tên
            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(paginationDTO.SearchTerm));
            }

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(paginationDTO.Category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name.Contains(paginationDTO.Category));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(paginationDTO.Status))
            {
                query = query.Where(p => p.Status == paginationDTO.Status);
            }

            // Lọc theo khoảng giá từ batch gần nhất
            if (paginationDTO.MinPrice.HasValue || paginationDTO.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.BatchDetails.OrderByDescending(b => b.CreateDate).Select(b => b.SellingPrice).FirstOrDefault() >= paginationDTO.MinPrice &&
                    p.BatchDetails.OrderByDescending(b => b.CreateDate).Select(b => b.SellingPrice).FirstOrDefault() <= paginationDTO.MaxPrice
                );
            }

            // Sắp xếp
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {
                    "name" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                    "createdate" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate),
                    _ => query.OrderByDescending(p => p.CreateDate)
                };
            }

            var product = await query.FirstOrDefaultAsync();
            if (product == null)
            {
                throw new Exception("Không tìm thấy sản phẩm phù hợp.");
            }

            var latestBatch = product.BatchDetails
                .OrderByDescending(b => b.CreateDate)
                .FirstOrDefault();

            var productDTO = new ProductUserListDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Status = product.Status,
                Language = product.Language,
                Cover = product.Cover,
                BoxId = product.BoxId,
                Category = product.Category?.Adapt<CategoryProductDTO>(),
                Images = product.Images?.Select(i => i.Adapt<ImageDTO>()).ToList() ?? new List<ImageDTO>(),
                BatchDetails = new List<BatchDetailUserProductDTO>()
        {
            latestBatch?.Adapt<BatchDetailUserProductDTO>()!
        }
            };

            return productDTO;
        }
        public async Task<Product?> GetProductByBoxIdAsync(int boxId)
        {
            return await _context.Products
                .Where(p => p.BoxId == boxId)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.BatchDetails)
                .FirstOrDefaultAsync();
        }
    }
}
