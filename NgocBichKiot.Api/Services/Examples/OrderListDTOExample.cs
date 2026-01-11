using Application.DTO.Orders;
using Application.DTO.Records;
using Application.DTO.Users;
using Swashbuckle.AspNetCore.Filters;

namespace NgocBichKiot.Api.Services.Examples
{
    public class OrderListDTOExample : IExamplesProvider<List<OrderListDTO>>
    {
        public List<OrderListDTO> GetExamples()
        {
            return new List<OrderListDTO>
            {
                new OrderListDTO
                {
                    Id = 16,
                    OrderAmount = 24000,
                    OrderDate = new DateTime(2025, 4, 16, 7, 0, 54, 203),
                    OrderStatus = "Pending",
                    OrderDetails = new List<OrderDetailss>
                    {
                        new OrderDetailss
                        {
                            ProductId = 1,
                            Quantity = 1,
                            UnitPrice = 12000,
                            TotalPrice = 12000,
                            
                            //Product = new ProductOrderDTO
                            //{
                            //    Id = 1,
                            //    Name = "7up",
                            //    //SellingPrice = 12000
                            //}
                        },
                        new OrderDetailss
                        {
                            ProductId = 2,
                            Quantity = 1,
                            UnitPrice = 12000,
                            TotalPrice = 12000,
                            //Product = new ProductOrderDTO
                            //{
                            //    Id = 2,
                            //    Name = "sting",
                            //    //SellingPrice = 12000
                            //}
                        }
                    },
                    Logs = new List<LogDTO>
                    {
                        new LogDTO
                        {
                            Id = 32,
                            ProductId = 1,
                            Quantity = 1,
                            Type = "Export",
                            
                        },
                        new LogDTO
                        {
                            Id = 33,
                            ProductId = 2,
                            Quantity = 1,
                            Type = "Export",
                           
                        }
                    }
                }
            };
        }
    }
}
