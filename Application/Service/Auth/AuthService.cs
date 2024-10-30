using Domain.Dto;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Domain.Class;
using Application.Interface;
using Domain.Dto.Responce;

namespace Application.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<Users> _passwordHasher;

        // Dependency Injection orqali barcha servislar kiritiladi
        public AuthService(
            ApplicationDbContext dbContext,
            ILogger<AuthService> logger,
            IConfiguration config,
            IPasswordHasher<Users> passwordHasher)
        {
            _dbContext = dbContext;
            _logger = logger;
            _config = config;
            _passwordHasher = passwordHasher;
        }

        // Foydalanuvchini ro'yxatdan o'tkazish metodi
        public async Task<ResponseDto<Users>> RegisterAsync(RegisterDto registerDto)
        {
            // Foydalanuvchini tekshirish (agar mavjud bo'lsa)
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username);

            if (existingUser != null)
            {
                _logger.LogWarning("User {Username} already exists.", registerDto.Username);
                return CreateResponse(null, "User already exists", false);
            }

            // Yangi foydalanuvchini yaratish
            var passwordHash = _passwordHasher.HashPassword(new Users(), registerDto.Password);
            var newUser = new Users
            {
                FullName = registerDto.FullName,
                Username = registerDto.Username,
                Password = passwordHash,
                Role = Role.Student,
                PhoneNumber = registerDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("User {Username} registered successfully.", newUser.Username);
            return CreateResponse(newUser, "User registered successfully", true);
        }

        // Foydalanuvchini tizimga kiritish metodi
        public async Task<ResponseDto<Users>> LoginAsync(LoginDto loginDto, long telegramId)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for username: {Username}", loginDto.Username);
                return CreateResponse(null, "Invalid username or password", false);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);

            if (result != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Invalid password for user: {Username}", loginDto.Username);
                return CreateResponse(null, "Invalid username or password", false);
            }

            var session = await _dbContext.Sessions.FindAsync(user.Id);

            if (session != null)
            {
                // Yangilash
                session.TelegramId = telegramId;
                session.CreatedAt = DateTime.UtcNow;
                session.ExpiredAt = DateTime.UtcNow.AddDays(5);
            }
            else
            {
                // Yangi sessiyani qo'shish
                session = new Sessions
                {
                    UserId = user.Id,
                    TelegramId = telegramId,
                    CreatedAt = DateTime.UtcNow, // Qo'shish vaqti
                    ExpiredAt = DateTime.UtcNow.AddDays(5)
                };
                await _dbContext.Sessions.AddAsync(session); // Async metoddan foydalanish
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("User {Username} logged in successfully.", loginDto.Username);
                return CreateResponse(user, "User logged in successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the session for user: {Username}", loginDto.Username);
                return CreateResponse(null, "Error occurred during login. Please try again.", false);
            }
        }


        // Yordamchi metod umumiy javobni qaytarish uchun
        private ResponseDto<Users> CreateResponse(Users user, string message, bool success)
        {
            return new ResponseDto<Users>
            {
                Data = user,
                Message = message,
                Success = success
            };
        }

        public async Task<ResponseDto<Users>> IsLoginAsync(long telegramId)
        {
            var islogin = await _dbContext.Sessions
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            if (islogin == null)
            {
                return new ResponseDto<Users>
                {
                    Message = "User not found",
                    Success = false
                };
            }
            else
            {
                if (islogin.ExpiredAt < DateTime.UtcNow)
                {
                    return new ResponseDto<Users>
                    {
                        Message = "Session expired",
                        Success = false
                    };
                }

                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == islogin.UserId);

                return new ResponseDto<Users>
                {
                    Success = true,
                    Message = "User found",
                    Data = user
                };

            }
        }
    }
}
