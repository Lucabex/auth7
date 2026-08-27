using Microsoft.AspNetCore.Mvc;
using auth7.Data;
using auth7.Models;
using auth7.DTO;
using Microsoft.EntityFrameworkCore;

namespace auth7.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context)
    {
        _context=context;
    }

    [HttpPost("reg")]
    public async Task<IActionResult> Reg(RegDto dto)
    {
        if(await _context.User.AnyAsync(u=> u.Name.ToLower() == dto.Name.ToLower()))
        {
            return BadRequest("User already in use");
        }
        var user = new User
        {
            Name=dto.Name,
            HashPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        
        _context.User.Add(user);
        await _context.SaveChangesAsync();

        var respons = new RegRes
        {
            Id=user.Id,
            Name=user.Name,
            
        };

        return Ok(respons);
    }

    [HttpPost("log")]
    public async Task<IActionResult> Log(LogDto dto)
    {
        var user = await _context.User.FirstOrDefaultAsync(u=> u.Name.ToLower() == dto.Name.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password , user.HashPassword))
        {
            return BadRequest("Invalid user name or password");
        }
        
        var respons = new LogRes
        {
            Id = user.Id,
            Name = user.Name,
            
        };
        return Ok(respons);
    }

}