    using Application.Commons;
using Application.DTO.BatchDetails;
using Application.DTO.News;
using Application.DTO.Products;
using Application.DTO.Records;
using Application.DTO.Users;
using Application.Interfaces;
using Application.Interfaces.Products;
using Application.IRepositories.Products;
using Domain.Enum;
using Domain.Model;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;       
       
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;                        
        }
        public async Task<Pagination<ProductListDTO>> GetPaginationAsync(ProductPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.ProductRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<ProductListDTO>
                {
                    Items = pagination.Items.Adapt<List<ProductListDTO>>(),
                    TotalItemsCount = pagination.TotalItemsCount,
                    PageSize = pagination.PageSize,
                    PageIndex = pagination.PageIndex
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting pagination: {ex.Message}", ex);
            }
        }

        public async Task<Pagination<ProductUserListDTO>> GetUserProductPaginationAsync(ProductPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.ProductRepo.GetUserProductPaginationAsync(paginationDTO);

                return new Pagination<ProductUserListDTO>
                {
                    Items = pagination.Items.Adapt<List<ProductUserListDTO>>(),
                    TotalItemsCount = pagination.TotalItemsCount,
                    PageSize = pagination.PageSize,
                    PageIndex = pagination.PageIndex
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting pagination: {ex.Message}", ex);
            }
        }

        public async Task<ProductUserListDTO> GetUserProductByIdWithFiltersAsync(int? productId, ProductPaginationDTO paginationDTO)
        {
            var product = await _unitOfWork.ProductRepo.GetUserProductByIdWithFiltersAsync(productId ?? 0, paginationDTO);

            if (product == null)
            {
                throw new Exception("Product is not existed");
            }

            return product;
        }
        public async Task<IEnumerable<ProductListDTO>> GetProductAsync()
        {
            var accounts = await _unitOfWork.ProductRepo.GetAllAsync(includeProperties: "Logs,Images,OrderDetails");
            return accounts.Adapt<IEnumerable<ProductListDTO>>();
        }
        public async Task<ProductListDTO> GetProductByIdAsync(int id)
        {
            var exist = await _unitOfWork.ProductRepo
                .GetById(id, includeProperties: "Logs,Images,OrderDetails,Category,BatchDetails");

            if (exist == null)
            {
                throw new Exception("Product is not existed");
            }

            var productDto = exist.Adapt<ProductListDTO>();

            // Filter Logs
            productDto.Logs = exist.Logs
                .Select(log => new ProductLogListDTO
                {
                    Id = log.Id,
                    ProductId = log.ProductId,
                    Name = log.Name,
                    Phone = log.Phone,
                    Address = log.Address,
                    Quantity = log.Quantity,
                    CreateDate = log.CreateDate,
                    Type = log.Type,

                    // Nếu là UpdatePrice thì giữ giá, còn lại bỏ qua
                    //OldOriginalPrice = log.Type == "UpdatePrice" ? log.OldOriginalPrice : null,
                    //NewOriginalPrice = log.Type == "UpdatePrice" ? log.NewOriginalPrice : null,
                    OldSellingPrice = log.Type == "UpdatePrice" ? log.OldSellingPrice : null,
                    NewSellingPrice = log.Type == "UpdatePrice" ? log.NewSellingPrice : null,
                    OldImportCost = log.Type == "UpdatePrice" ? log.OldImportCost : null,
                    NewImportCost = log.Type == "UpdatePrice" ? log.NewImportCost : null
                })
                .ToList();

            return productDto;
        }

        public async Task<ProductUserListDTO> GetProductUserByIdAsync(int id)
        {
            var exist = await _unitOfWork.ProductRepo
                .GetById(id, includeProperties: "Logs,Images,OrderDetails,Category,BatchDetails");

            if (exist == null)
                throw new Exception("Product is not existed");

            var batchDetails = await _unitOfWork.BatchDetailRepo.GetBatchDetailsByProductIdAsync(id);

            var result = exist.Adapt<ProductUserListDTO>();

            result.BatchDetails = batchDetails.Take(1).ToList();

            return result;
        }
        public async Task<ProductListDTO> CreateProduct(CreateProductDTO createProductDto)
        {
            try
            {
                var category = await _unitOfWork.CateRepo.GetByIdAsync(createProductDto.CategoryId);
                if (category == null || category.CateType != "Product")
                {
                    throw new Exception("Loại sản phẩm không hợp lệ!");
                }

                Product product;

                if (createProductDto.BoxId.HasValue)
                {
                    // Tạo sản phẩm con dựa trên sản phẩm cha (box)
                    var parentProduct = await _unitOfWork.ProductRepo.GetByIdAsync(createProductDto.BoxId.Value);
                    if (parentProduct == null)
                        throw new Exception("Sản phẩm cha (box) không tồn tại!");

                    product = createProductDto.Adapt<Product>();
                    product.BoxId = parentProduct.Id;
                    product.PacksPerUnit = 0; // giả sử sản phẩm con không chứa hộp khác

                    await _unitOfWork.ProductRepo.AddAsync(product);
                    await _unitOfWork.SaveChangeAsync();

                    // Thêm ảnh cho sản phẩm con nếu có
                    if (createProductDto.ProductImages != null && createProductDto.ProductImages.Any())
                    {
                        foreach (var url in createProductDto.ProductImages)
                        {
                            var image = new Image
                            {
                                UrlPath = url,
                                ProductId = product.Id
                            };
                            await _unitOfWork.ImageRepo.AddAsync(image);
                        }
                    }
                }
                else
                {
                    // Tạo sản phẩm chính (không nằm trong hộp)
                    product = createProductDto.Adapt<Product>();
                    product.BoxId = null;

                    await _unitOfWork.ProductRepo.AddAsync(product);
                    await _unitOfWork.SaveChangeAsync();

                    // Thêm ảnh nếu có
                    if (createProductDto.ProductImages != null && createProductDto.ProductImages.Any())
                    {
                        foreach (var url in createProductDto.ProductImages)
                        {
                            var image = new Image
                            {
                                UrlPath = url,
                                ProductId = product.Id
                            };
                            await _unitOfWork.ImageRepo.AddAsync(image);
                        }
                    }
                }

                await _unitOfWork.SaveChangeAsync();

                return product.Adapt<ProductListDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating product: {ex.Message}", ex);
            }
        }

        public async Task<ProductListDTO> BreakBox(CreateChildProductDTO createProductDto)
        {
            try
            {
                var parentProduct = await _unitOfWork.ProductRepo.GetByIdAsync(createProductDto.BoxId);
                if (parentProduct == null)
                {
                   throw new Exception("Sản phẩm cha không tồn tại!");
                }

                var product = await _unitOfWork.ProductRepo.GetProductByBoxIdAsync(createProductDto.BoxId);
                if (product == null)
                {
                    throw new Exception("Sản phẩm con ko tồn tại trong hệ thống!");
                }

                var bacthdetail = await _unitOfWork.BatchDetailRepo.GetByIdAsync(createProductDto.BatchDetailParentId);
                if (bacthdetail == null)
                {
                    throw new Exception("Batch detail parent không tồn tại!");
                }
                

                if (createProductDto.isAddMorePacks == true)
                {
                    bacthdetail.RemainingQuantity += createProductDto.Quantity;
                    _unitOfWork.BatchDetailRepo.Update(bacthdetail);
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    if (bacthdetail.RemainingQuantity <= 0)
                    {
                        throw new Exception("Số lượng không đủ để tách!");
                    }
                    bacthdetail.RemainingQuantity -= createProductDto.Quantity;
                    _unitOfWork.BatchDetailRepo.Update(bacthdetail);
                    await _unitOfWork.SaveChangeAsync();
                }
                

                
                var newbacthdetail = new BatchDetail
                {
                    BatchDetailParentId = bacthdetail.Id, 
                    ProductId = product.Id,
                    Quantity = createProductDto.Quantity * parentProduct.PacksPerUnit, 
                    SellingPrice = createProductDto.SellingPrice,
                    ImportCosts = bacthdetail.ImportCosts,
                    SourceOfProductName = bacthdetail.SourceOfProductName,
                    RemainingQuantity = createProductDto.Quantity * parentProduct.PacksPerUnit,
                    BatchId = bacthdetail.BatchId,
                   
                };
                await _unitOfWork.BatchDetailRepo.AddAsync(newbacthdetail);
                await _unitOfWork.SaveChangeAsync();

                var log = new Log
                {
                    ProductId = product.Id,
                    Quantity = newbacthdetail.RemainingQuantity, 
                    Type = createProductDto.isAddMorePacks == true ? "AddMorePack" : "BreakBox",
                    OldSellingPrice = bacthdetail.SellingPrice,
                    NewSellingPrice = newbacthdetail.SellingPrice,
                    BatchDetailId = newbacthdetail.Id, 
                    BatchId = bacthdetail.BatchId, 
                };
                await _unitOfWork.LogRepo.AddAsync(log);
                var parentLog = new Log
                {
                    ProductId = parentProduct.Id,
                    Quantity = createProductDto.Quantity,
                    Type = "BreakBox",
                    //OldSellingPrice = bacthdetail.SellingPrice,
                    //NewSellingPrice = bacthdetail.SellingPrice, // giữ nguyên vì không đổi giá
                    BatchDetailId = bacthdetail.Id,
                    BatchId = bacthdetail.BatchId
                };
                await _unitOfWork.LogRepo.AddAsync(parentLog);

                await _unitOfWork.SaveChangeAsync();
                return product.Adapt<ProductListDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while create product: {ex.Message}", ex);
            }
        }
        //public async Task<ProductListDTO> CreateProduct(CreateProductDTO createProductDto)
        //{
        //    try
        //    {
        //        var product = createProductDto.Adapt<Product>();              
        //        await _unitOfWork.ProductRepo.AddAsync(product);
        //        await _unitOfWork.SaveChangeAsync();

        //        // Upload multiple images
        //        if (createProductDto.ProductImages != null && createProductDto.ProductImages.Any())
        //        {
        //            foreach (var image in createProductDto.ProductImages)
        //            {
        //                var (publicId, url) = await _cloudinaryService.UploadFileAsync(image, "products");

        //                var productImage = new Image
        //                {
        //                    UrlPath = url,
        //                    ProductId = product.Id,

        //                };

        //                await _unitOfWork.ImageRepo.AddAsync(productImage);
        //            }
        //            await _unitOfWork.SaveChangeAsync();
        //        }

        //        var log = new Log
        //        {
        //            ProductId = product.Id,
        //            Name = createProductDto.UserName,
        //            Phone = createProductDto.Phone,
        //            Address = createProductDto.Address,                   
        //            Quantity = createProductDto.StockQuantity,
        //            Type = LogType.Import.ToString()                   
        //        };
        //        await _unitOfWork.LogRepo.AddAsync(log);
        //        await _unitOfWork.SaveChangeAsync();

        //        return product.Adapt<ProductListDTO>();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating product");
        //        throw;
        //    }
        //}




        //public async Task<ProductListDTO> UpdateProductAsync(int id, UpdateProductDTO productDTO)
        //{
        //    try
        //    {
        //        var existingProduct = await _unitOfWork.ProductRepo.GetByIdAsync(id);
        //        if (existingProduct == null)
        //            throw new Exception("Product is not existed");

        //        if (existingProduct.IsDeleted == true)
        //            throw new Exception("Product is deleted in system");

        //        // Lấy danh sách ảnh hiện tại
        //        var currentImages = await _unitOfWork.ImageRepo.GetAllAsync(x => x.ProductId == id);

        //        // Xóa ảnh cũ
        //        if (productDTO.ImageIdsToDelete != null && productDTO.ImageIdsToDelete.Any())
        //        {
        //            foreach (var imageId in productDTO.ImageIdsToDelete)
        //            {
        //                var imageToDelete = currentImages.FirstOrDefault(x => x.Id == imageId && x.ProductId == id);
        //                if (imageToDelete != null)
        //                {
        //                    if (!string.IsNullOrEmpty(imageToDelete.UrlPath))
        //                    {
        //                        var publicId = _cloudinaryService.GetPublicIdFromUrl(imageToDelete.UrlPath);
        //                        await _cloudinaryService.DeleteFileAsync(publicId);
        //                    }
        //                    await _unitOfWork.ImageRepo.DeleteAsync(imageToDelete);
        //                }
        //            }
        //        }

        //        // Upload ảnh mới
        //        if (productDTO.ProductImages != null && productDTO.ProductImages.Any())
        //        {
        //            var images = new List<Image>();
        //            foreach (var image in productDTO.ProductImages)
        //            {
        //                var (publicId, url) = await _cloudinaryService.UploadFileAsync(image, "products");
        //                images.Add(new Image { UrlPath = url, ProductId = id });
        //            }
        //            await _unitOfWork.ImageRepo.AddRangeAsync(images);
        //        }

        //        // So sánh giá để lưu log nếu có thay đổi
        //        bool isImportChanged = productDTO.ImportCosts != existingProduct.ImportCosts;
        //        bool isOriginalChanged = productDTO.OriginalPrice != existingProduct.OriginalPrice;
        //        bool isSellingChanged = productDTO.SellingPrice != existingProduct.SellingPrice;

        //        if (isImportChanged || isOriginalChanged || isSellingChanged)
        //        {
        //            var log = new Log
        //            {
        //                ProductId = existingProduct.Id,
        //                Type = "UpdatePrice",
        //                CreateDate = DateTime.UtcNow,

        //                Name = productDTO.UpdatedByName,
        //                Phone = productDTO.UpdatedByPhone,
        //                Address = productDTO.UpdatedByAddress
        //            };

        //            if (isImportChanged)
        //            {
        //                log.OldImportCost = existingProduct.ImportCosts;
        //                log.NewImportCost = productDTO.ImportCosts;
        //            }

        //            if (isOriginalChanged)
        //            {
        //                log.OldOriginalPrice = existingProduct.OriginalPrice;
        //                log.NewOriginalPrice = productDTO.OriginalPrice;
        //            }

        //            if (isSellingChanged)
        //            {
        //                log.OldSellingPrice = existingProduct.SellingPrice;
        //                log.NewSellingPrice = productDTO.SellingPrice;
        //            }

        //            await _unitOfWork.LogRepo.AddAsync(log);
        //            await _unitOfWork.SaveChangeAsync();
        //        }

        //        // Cập nhật sản phẩm
        //        _unitOfWork.ProductRepo.Update(productDTO.Adapt(existingProduct));
        //        await _unitOfWork.SaveChangeAsync();

        //        var updated = await _unitOfWork.ProductRepo.GetById(id, includeProperties: "Logs,Images");
        //        return updated.Adapt<ProductListDTO>();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating product");
        //        throw;
        //    }
        //}
        public async Task<ProductListDTO> UpdateProductAsync(int id, UpdateProductDTO productDTO)
        {
            try
            {
                var existingCate = await _unitOfWork.CateRepo.GetByIdAsync(productDTO.CategoryId);
                if (existingCate == null)
                {
                    throw new Exception("Category does not exist");
                }
                //var existingSource = await _unitOfWork.SourceRepo.GetByIdAsync(productDTO.SourceOfProductId);
                //if (existingSource == null)
                //{
                //    throw new Exception("Source of product does not exist");
                //}
                var existingProduct = await _unitOfWork.ProductRepo.GetByIdAsync(id);
                if (existingProduct == null)
                    throw new Exception("Sản phẩm không tồn tại!");

                if (existingProduct.IsDeleted == true)
                    throw new Exception("Sản phẩm đã bị xóa khỏi hệ thống!");

                // So sánh giá để lưu log nếu có thay đổi
               // bool isImportChanged = productDTO.ImportCosts != existingProduct.ImportCosts;
               // bool isOriginalChanged = productDTO.OriginalPrice != existingProduct.OriginalPrice;
                //bool isSellingChanged = productDTO.SellingPrice != existingProduct.SellingPrice;

                //if (isImportChanged /*|| isOriginalChanged*/ || isSellingChanged)
                //{
                //    var log = new Log
                //    {
                //        ProductId = existingProduct.Id,
                //        Type = LogType.UpdatePrice.ToString(),
                //        CreateDate = DateTime.UtcNow,
                //        Name = productDTO.UpdatedByName,
                //        Phone = productDTO.UpdatedByPhone,
                //        Address = productDTO.UpdatedByAddress
                //    };

                //    if (isImportChanged)
                //    {
                //        log.OldImportCost = existingProduct.ImportCosts;
                //        log.NewImportCost = productDTO.ImportCosts;
                //    }

                //    //if (isOriginalChanged)
                //    //{
                //    //    log.OldOriginalPrice = existingProduct.OriginalPrice;
                //    //    log.NewOriginalPrice = productDTO.OriginalPrice;
                //    //}

                //    if (isSellingChanged)
                //    {
                //        log.OldSellingPrice = existingProduct.SellingPrice;
                //        log.NewSellingPrice = productDTO.SellingPrice;
                //    }

                //    await _unitOfWork.LogRepo.AddAsync(log);
                //    await _unitOfWork.SaveChangeAsync();
                //}

                // Cập nhật thông tin sản phẩm
                _unitOfWork.ProductRepo.Update(productDTO.Adapt(existingProduct));
                await _unitOfWork.SaveChangeAsync();

                // Xử lý cập nhật ảnh (XÓA CỨNG ảnh không còn dùng, THÊM ảnh mới)
                if (productDTO.ProductImages != null)
                {
                    var existingImages = await _unitOfWork.ImageRepo.GetAllAsync(x => x.ProductId == id);
                    var existingUrls = existingImages.Select(i => i.UrlPath).ToList();
                    var newUrls = productDTO.ProductImages;

                    // 1. Xóa ảnh không còn được sử dụng
                    var toDelete = existingImages.Where(i => !newUrls.Contains(i.UrlPath)).ToList();
                    foreach (var img in toDelete)
                    {
                        await _unitOfWork.ImageRepo.DeleteAsync(img);
                    }

                    // 2. Thêm ảnh mới
                    var toAdd = newUrls.Where(url => !existingUrls.Contains(url)).ToList();
                    foreach (var url in toAdd)
                    {
                        var image = new Image
                        {
                            UrlPath = url,
                            ProductId = id,                                                  
                        };
                        await _unitOfWork.ImageRepo.AddAsync(image);
                    }

                    await _unitOfWork.SaveChangeAsync();
                }

                var updated = await _unitOfWork.ProductRepo.GetById(id, includeProperties: "Logs");
                return updated.Adapt<ProductListDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while update product: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateStock(int productId, int quantity, string transactionType, UserInforDTO userInfor)
        {
            // 1️⃣ Lấy sản phẩm từ DB
            var product = await _unitOfWork.ProductRepo.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Không tìm thấy sản phẩm.");
            }

            // 2️⃣ Kiểm tra loại giao dịch (Nhập / Xuất)
            //if (transactionType == LogType.Import.ToString()) // Nhập hàng
            //{
            //    product.StockQuantity += quantity;
            //}
            //else if (transactionType == LogType.Export.ToString()) // Xuất hàng
            //{
            //    if (product.StockQuantity < quantity)
            //    {
            //        throw new Exception("Không đủ số lượng tồn kho!.");
            //    }
            //    product.StockQuantity -= quantity;
            //}
            //else
            //{
            //    throw new Exception("Invalid transaction type.");
            //}

            // 3️⃣ Cập nhật lại sản phẩm
            _unitOfWork.ProductRepo.Update(product);
            await _unitOfWork.SaveChangeAsync();
            
            // 4️⃣ Ghi Log lịch sử
            var log = new Log
            {
                ProductId = product.Id,
                Name = userInfor.Name,
                Phone = userInfor.Phone,
                Address = userInfor.Address,
                //UserId = _claimsService.GetCurrentUserId,
                Quantity = quantity,
                Type = transactionType,

            };

            await _unitOfWork.LogRepo.AddAsync(log);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
        //public async Task<ProductDTO> UpdateProductQuantityAsync(int id, UpdateProductQuantity updateProductDTO)
        //{
        //    var existingProduct = await _unitOfWork.ProductRepo.GetByIdAsync(id);
        //    if (existingProduct == null)
        //    {
        //        throw new Exception("Product does not exist.");
        //    }
        //    if (existingProduct.IsDeleted)
        //    {
        //        throw new Exception("Product is deleted from the system.");
        //    }

        //    // Tính toán sự thay đổi số lượng tồn kho
        //    int? quantityDifference = updateProductDTO.StockQuantity - existingProduct.StockQuantity;
        //    string transactionType = quantityDifference > 0 ? LogType.Import.ToString() : LogType.Export.ToString();

        //    // Cập nhật thông tin sản phẩm
        //    _unitOfWork.ProductRepo.Update(updateProductDTO.Adapt(existingProduct));
        //    await _unitOfWork.SaveChangeAsync();

        //    // Ghi log nếu số lượng tồn kho thay đổi
        //    if (quantityDifference != 0)
        //    {
        //        var log = new Log
        //        {
        //            ProductId = existingProduct.Id,
        //            UserId = _claimsService.GetCurrentUserId,
        //            Quantity = Math.Abs((int)quantityDifference), // Luôn lưu số dương
        //            Type = transactionType
        //        };

        //        await _unitOfWork.LogRepo.AddAsync(log);
        //        await _unitOfWork.SaveChangeAsync();
        //    }

        //    return existingProduct.Adapt<ProductDTO>();
        //}

        public async Task DeleteOrEnable(int productId, bool isDeleted)
        {
            var account = await _unitOfWork.ProductRepo.GetAsync(d => d.Id == productId);
            if (account is null)
            {
                throw new Exception("Product is not existed");
            }
            account.Status = isDeleted
            ? ProductStatus.UnAvailable.ToString()
            : ProductStatus.Available.ToString();
            account.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
