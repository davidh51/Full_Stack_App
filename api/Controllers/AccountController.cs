using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)  //Data validation -> ControllerBase
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username!.ToLower());
            if (user == null)
                return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, false);
            if (!result.Succeeded)
                return Unauthorized("Username not found and/or invalid password");
                
            return Ok(new NewUserDto
                    {
                        UserName = user.UserName!,
                        Email = user.Email!,
                        Token = _tokenService.CreateToken(user)
                    });
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)  //Data validation -> ControllerBase
                    return BadRequest(ModelState);
                var AppUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                }; 
                var createdUser = await _userManager.CreateAsync(AppUser, registerDto.Password!);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(AppUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new NewUserDto
                        {
                            UserName = AppUser.UserName!,
                            Email = AppUser.Email!,
                            Token = _tokenService.CreateToken(AppUser)
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}