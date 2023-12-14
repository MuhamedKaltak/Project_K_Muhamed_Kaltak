using DatabaseAccess;
using DatabaseAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Services
{
    public class DatabaseUserServiceEntityFramework
    {
        public async Task AddUser(User user)
        {
            await using Context context = new Context();

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckExistingUserByEmail(string email)
        {
            await using Context context = new Context();

            if (await context.Users.AnyAsync(u => u.Email == email))
                return true;
            else
                return false;
       
        }

        public async Task<bool> CheckExistingUserByUsername(string username)
        {
            await using Context context = new Context();

            if (await context.Users.AnyAsync(u => u.Username == username))
                return true;
            else
                return false;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            await using Context context = new Context();

            return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            await using Context context = new Context();

            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUser(User user) //Möljigen att jag ändrar detta för det är inte full async
        {
            await using Context context = new Context();

            context.Users.Entry(user).State = EntityState.Modified;

            await context.SaveChangesAsync();
        }
    }
}
