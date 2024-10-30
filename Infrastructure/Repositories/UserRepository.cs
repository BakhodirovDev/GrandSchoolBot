using Domain.Class;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly DbContext _context;

        public UserRepository(DbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Users user)
        {
            await _context.Set<Users>().AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Users> GetByChatIdAsync(long chatId)
        {
            return await _context.Set<Users>().FirstOrDefaultAsync(u => u.ChatId == chatId);
        }

        public async Task UpdateAsync(Users user)
        {
            _context.Set<Users>().Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Users user)
        {
            _context.Set<Users>().Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
