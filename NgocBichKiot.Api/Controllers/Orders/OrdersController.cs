using Application.DTO.Orders;
using Application.DTO.Products;
using Application.Interfaces.Oders;
using Application.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NgocBichKiot.Api.Controllers.Products;
using NgocBichKiot.Api.Services.Examples;
using OfficeOpenXml;
using Swashbuckle.AspNetCore.Filters;
using System.Drawing.Printing;
using System.Text;
using QuestPDF.Fluent;

namespace NgocBichKiot.Api.Controllers.Orders
{
    
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;
       
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
           
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrderListDTOExample))]
        public async Task<IActionResult> GetOrderPagination([FromQuery] OrderPaginationDTO paginationDTO)
        {
            try
            {
                var result = await _orderService.GetPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrderListDTOExample))]       
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            var findAccountUser = await _orderService.GetOrderByIdAsync(id);
            return Ok(findAccountUser);
        }
        [HttpGet()]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetMyOrdersAsync();
            return Ok(orders);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createProduct)
        {
            var response = await _orderService.CreateOrderAsync(createProduct);
            return Created(nameof(CreateOrder), response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrderFromCart(int addressId, string paymentmethod, string note)
        {
            var response = await _orderService.CreateOrderFromCartAsync(addressId,  paymentmethod,  note);
            return Created(nameof(CreateOrderFromCart), response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int id,  string orderStatus)
        {
            var updatedProd = await _orderService.UpdateOrderStatus(id, orderStatus);
            return Ok(updatedProd);
        }
        [HttpGet]
        public async Task<IActionResult> CheckOut(int orderId, string paymentMethod)
        {
            try
            {
                var checkoutDTO = await _orderService.CheckOut(orderId, paymentMethod);

                var document = new CheckOutInvoiceDocument(checkoutDTO);
                var stream = new MemoryStream();
                document.GeneratePdf(stream);
                stream.Position = 0;
                string fileName = $"Invoice_Order_{orderId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                return File(stream, "application/pdf", fileName);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDTO updateOrder)
        {
            var updatedProd = await _orderService.UpdateOrderAsync(id, updateOrder);
            return Ok(updatedProd);
        }
    }
}
