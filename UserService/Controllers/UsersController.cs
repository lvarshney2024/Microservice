using Microsoft.AspNetCore.Mvc;
using UserService.Dtos;
using UserService.Models;
using UserService.Services;
using System.Collections.Generic;

namespace UserService.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("CreateUser")]
        public ActionResult<User> Create([FromBody] CreateUserDto dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("GetUser/{id}")]
        public ActionResult<User> GetById(string id)
        {
            var user = _service.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("GetUserList")]               
        public ActionResult<List<User>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        //[HttpPut("{id}")]
        //public IActionResult Update(string id, [FromBody] CreateUserDto dto)
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
