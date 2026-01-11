using Application.DTO.Images;
using Application.DTO.Products;
using Application.DTO.Records;
using Swashbuckle.AspNetCore.Filters;

namespace NgocBichKiot.Api.Services.Examples
{
    public class ProductListDTOExample : IExamplesProvider<List<ProductListDTO>>
    {
        public List<ProductListDTO> GetExamples()
        {
            return new List<ProductListDTO>
            {
                new ProductListDTO
                {
                    Id = 10,
                    Name = "string",
                    //Category = "string",
                   // OriginalPrice = 0,
                    //SellingPrice = 0,
                    //SourceOfProducts = "string",
                    //ImportCosts = 0,
                    //StockQuantity = 0, 
                    Status = "string",     
                    Images = new List<ImageDTO>
                    {
                        new ImageDTO
                        {
                            Id = 17,
                            UrlPath = "https://res.cloudinary.com/ducxotvyo/image/upload/v1744784611/ngocbichkiot/products/yshclxpb27nxxno2ltdc.jpg"
                        }
                    },
                    Logs = new List<ProductLogListDTO>
                    {
                        new ProductLogListDTO
                        {
                            Id = 26,
                            ProductId = 10,
                            Quantity = 0,
                            Name = "string",
                            Phone = "number",
                            Address = "string",
                            Type = "Import",
                            
                        },
                        new ProductLogListDTO
                        {
                            Id = 26,
                            ProductId = 10,
                            Name = "string",
                            Phone = "number",
                            Address = "string",
                            Quantity = 0,
                            Type = "Export",
                            
                        }
                    },

                }
            };
        }
    }
}
