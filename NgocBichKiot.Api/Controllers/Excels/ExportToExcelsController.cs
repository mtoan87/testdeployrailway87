using Application.DTO.Dashboards;
using Application.Interfaces.Oders;
using Application.IRepositories.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace NgocBichKiot.Api.Controllers.Excels
{
    
    public class ExportToExcelsController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IProductRepo _productRepository;
        public ExportToExcelsController(IOrderService orderService, IProductRepo productRepository)
        {
            _orderService = orderService;
            _productRepository = productRepository;
        }
        [HttpGet()]
        [Authorize(Roles = "1")]
        public IActionResult Exports(DateTime? fromDate, DateTime? toDate)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // ✅ đúng

            var data = _orderService.orderExports(fromDate, toDate);

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Orders");

            // Header
            //sheet.Cells[1, 1].Value = "Tên khách hàng";
            sheet.Cells[1, 1].Value = "Tên món hàng";
            sheet.Cells[1, 2].Value = "Đơn vị";
            sheet.Cells[1, 3].Value = "Nguồn gốc";
            sheet.Cells[1, 4].Value = "Số lượng";
            sheet.Cells[1, 5].Value = "Đơn giá";
            sheet.Cells[1, 6].Value = "Thành tiền";
            sheet.Cells[1, 7].Value = "Ngày Quyết Toán";

            int row = 2;
            foreach (var item in data)
            {
                //sheet.Cells[row, 1].Value = item.CustomerName;
                sheet.Cells[row, 1].Value = item.ProductName;
                sheet.Cells[row, 2].Value = item.Unit;
                //sheet.Cells[row, 3].Value = item.SourceOfProduct;
                sheet.Cells[row, 3].Value = item.Quantity;
                sheet.Cells[row, 4].Value = item.UnitPrice;
                sheet.Cells[row, 5].Value = item.TotalPrice;
                sheet.Cells[row, 6].Value = item.OrderDate?.ToString("dd/MM/yyyy");
                row++;
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            var stream = new MemoryStream(package.GetAsByteArray());

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Quyết Toán.xlsx");
        }

        [HttpPost()]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ExportInventoryReport([FromBody] InventoryExportRequest request)
        {
            var file = await _productRepository.ExportInventoryReportAsync(request);
            var fileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
