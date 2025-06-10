using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.DTOs.Input;
using AppCreditosBackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppCreditosBackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<UserController>
        [HttpGet("clients")]
        public async Task<ActionResult<List<UserDTO>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }        // GET api/<UserController>/5
        [HttpGet("clients/{id}")]
        public async Task<ActionResult<UserDTO>> GetById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        // POST api/<UserController>
        [HttpPost("clients")]
        public async Task<ActionResult<UserDTO>> CreateUserAsync([FromBody] RegisterClientRequestDto value)
        {
            try
            {
                var user = await _userService.CreateUserAsync(value);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user: {ex.Message}");
            }
        }        // PUT api/<UserController>/5
        [HttpPut("clients/{id}")]
        public async Task<ActionResult<UserDTO>> UpdateUserAsync(Guid userid, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(userid, dto);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }// DELETE api/<UserController>/5
        [HttpDelete("clients/{id}")]
        public async Task<ActionResult> DeleteUserAsync(Guid id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
