using Domain.Class;
using Domain.Dto;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.User
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddUserAsync(UserDto userDto)
        {
            var user = new Users
            {
                FullName = $"{userDto.LastName} {userDto.LastName}",
                Username = userDto.Username,
                ChatId = userDto.TelegramId,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }

        public async Task<Users> GetUserByChatIdAsync(long chatId)
        {
            return await _userRepository.GetByChatIdAsync(chatId);
        }

        public async Task UpdateUserAsync(UserDto userDto, long chatId)
        {
            var user = await _userRepository.GetByChatIdAsync(chatId);
            if (user != null)
            {
                user.FullName = $"{userDto.LastName} {userDto.LastName}";
                user.Username = userDto.Username;

                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task DeleteUserAsync(long chatId)
        {
            var user = await _userRepository.GetByChatIdAsync(chatId);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user);
            }
        }
    }
}
