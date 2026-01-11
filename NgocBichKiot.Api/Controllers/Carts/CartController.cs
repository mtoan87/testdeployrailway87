using Application.DTO.Carts;
using Application.Interfaces.Carts;
using Microsoft.AspNetCore.Mvc;

namespace NgocBichKiot.Api.Controllers.Carts
{
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/Cart/display
        [HttpGet()]
        public async Task<IActionResult> GetUserCartDisplay()
        {            
            try
            {
                var result = await _cartService.GetUserCartDisplayAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CreateCartDTO create)
        {
           
            try
            {
                await _cartService.CreateCart(create);
                return Ok(new { message = "Cart item created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //try
        //   {
        //       await _productService.DeleteOrEnable(productId, isDeleted > 0);
        //       return NoContent();
        //   }
        //   catch (Exception ex)
        //   {
        //       return StatusCode(500, new { message = ex.Message });
        //   }
        // GET: api/Cart/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            try
            {
                var cart = await _cartService.GetCartById(id);
                if (cart == null)
                    return NotFound();

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

     
        [HttpPut()]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] UpdateCartItemDTO updateCart)
        {
            var updated = await _cartService.UpdateCartItemAsync(updateCart);
            return Ok(updated);
        }


        // PUT: api/Cart/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] UpdateCartDTO updateCart)
        {
            var updated = await _cartService.UpdateCart(id, updateCart);
            return Ok(updated);
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetCarts();
            return Ok(carts);
        }

        // DELETE (soft): api/Cart/{id}?isDeleted=true
        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteOrEnable(int id, [FromQuery] bool isDeleted)
        {
            await _cartService.DeleteOrEnable(id, isDeleted);
            return Ok(new { message = isDeleted ? "Cart item deleted" : "Cart item restored" });
        }

        [HttpDelete("productId")]
        public async Task<IActionResult> DeleteCartItem(int productId)
        {
            try
            {
                await _cartService.DeleteCartItemAsync(productId);
                return Ok(new { message = "Xóa sản phẩm khỏi giỏ hàng thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
