using Application.DTO.Orders;
using Application.DTO.Records;
using Swashbuckle.AspNetCore.Filters;

namespace NgocBichKiot.Api.Services.Examples
{
    public class GetListLogDTOExample : IExamplesProvider<List<LogListDTO>>
    {
        public List<LogListDTO> GetExamples()
        {
            return new List<LogListDTO>
            {
                new LogListDTO
                {
                    Id = 34,
                    ProductId = 9,
                    Name = "Duy",
                    Phone = "03646282612",
                    Address = "1231",
                    Quantity = 1,
                    Type = "Export",
                    Product = new ProductOrderDTO
                    {
                        Id = 9,
                        Name = "PEPSI",
                        //SellingPrice = 12000
                    }
                },
                new LogListDTO
                {
                    Id = 35,
                    ProductId = 10,
                    Name = "John",
                    Phone = "0987654321",
                    Address = "789 Sample Rd",
                    Quantity = 10,
                    Type = "Import",
                    Product = new ProductOrderDTO
                    {
                        Id = 10,
                        Name = "Coca-Cola",
                        //SellingPrice = 15000
                    }
                },
                new LogListDTO
                {
                    Id = 36,
                    ProductId = 11,
                    Name = "Jane",
                    Phone = "1231231234",
                    Address = "456 Example Blvd",
                    Quantity = 5,
                    Type = "Export",
                    Product = new ProductOrderDTO
                    {
                        Id = 11,
                        Name = "Sprite",
                        //SellingPrice = 10000
                    }
                }
            };
        }
    }
}
