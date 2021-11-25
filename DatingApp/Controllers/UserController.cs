using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{

    public class UserController : BaseApiController
    {
        private readonly DataContext context;
        private readonly IUserRepository userRepository;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public UserController(DataContext context,
            IUserRepository userRepository,
            ITokenService tokenService,
            IMapper mapper)
        {
            this.context = context;
            this.userRepository = userRepository;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            return Ok(await userRepository.GetMembersAsync());
        }

        [HttpGet("{username}")]
        [Authorize]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await userRepository.GetMembersByUsernameAsync(username);
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username already Exists");
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UserName = registerDto.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            context.Users.Add(user);

            await context.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.UserName,
                Token = tokenService.createToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null)
            {
                return Unauthorized("Invalid UserName");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                UserName = user.UserName,
                Token = tokenService.createToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userRepository.GetUserByUsernameAsync(username);
            mapper.Map(memberUpdateDto, user);

            userRepository.Update(user);

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Fail to Update User");

        }
    }
}
