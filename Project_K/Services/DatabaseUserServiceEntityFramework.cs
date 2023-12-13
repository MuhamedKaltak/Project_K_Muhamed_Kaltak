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
    }
}
