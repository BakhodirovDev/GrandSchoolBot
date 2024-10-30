using Domain.Class;
using Domain.Dto;
using Domain.Dto.Responce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IAuthService
    {
        Task<ResponseDto<Users>> IsLoginAsync(long telegramId);
        Task<ResponseDto<Users>> RegisterAsync(RegisterDto registerDto);
        Task<ResponseDto<Users>> LoginAsync(LoginDto loginDto, long telegramId);
    }
}
