using Application.DTO.Dashboards;
using Application.DTO.OrderDetails;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.OrderDetails
{
    public interface IOrderDetailRepo : IGenericRepository<OrderDetail>
    {
        Task<List<TopCustomerFromOrderDetailDTO>> GetTop5CustomersFromOrderDetailAsync();
        Task<int> GetTotalSoldProductsAsync(int year);

       
    }
}
