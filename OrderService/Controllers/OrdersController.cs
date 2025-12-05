using Microsoft.AspNetCore.Mvc;
using MicroService.Dtos;
using MicroService.Models;
using MicroService.Services;
using System.Collections.Generic;

namespace MicroService.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        //[HttpPost]
        //public ActionResult<Order> Create([FromBody] CreateOrderDto dto)
        //{
        //    var created = _service.Create(dto);
        //    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        //}

        //[HttpGet("{id}")]
        //public ActionResult<Order> GetById(string id)
        //{
        //    var order = _service.GetById(id);
        //    if (order == null) return NotFound();
        //    return Ok(order);
        //}

        [Route("GetOrderList")]
        [HttpGet]
        public ActionResult<List<Order>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        //[HttpPut("{id}")]
        //public IActionResult Update(string id, [FromBody] CreateOrderDto dto)
        //{
        //    _service.Update(id, dto);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public IActionResult Delete(string id)
        //{
        //    _service.Delete(id);
        //    return NoContent();
        //}
    }
}
