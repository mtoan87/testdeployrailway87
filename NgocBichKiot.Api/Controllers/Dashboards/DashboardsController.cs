using Application.Interfaces.Dashboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NgocBichKiot.Api.Controllers.Dashboards
{
    
    public class DashboardsController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        public DashboardsController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet()]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetDashboardData(int year)
        {
            try
            {
                var dashboard = await _dashboardService.GetDashboardDataAsync(year); 
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        //[HttpGet()]
        //public async Task<IActionResult> GetTopSellingProducts()
        //{
        //    var result = await _dashboardService.GetTop5BestSellingProductsAsync();
        //    return Ok(result);
        //}

        //[HttpGet()]
        //public async Task<IActionResult> GetRevenueByYear()
        //{
        //    var result = await _dashboardService.GetRevenueByYearAsync();
        //    return Ok(result);
        //}
        //[HttpGet()]
        //public async Task<IActionResult> GetMonthlyOrderStats(int year)
        //{
        //    var result = await _dashboardService.GetMonthlyOrderStatsAsync(year);
        //    return Ok(result);
        //}
        //[HttpGet()]
        //public async Task<IActionResult> GetTopCustomers( )
        //{
        //    var result = await _dashboardService.GetTop5CustomersFromOrderDetailAsync();
        //    return Ok(result);
        //}
    }
}
