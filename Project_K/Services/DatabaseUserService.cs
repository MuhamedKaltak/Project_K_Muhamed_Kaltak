using Project_K.Utilities;
using Project_K.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Services
{
    public class DatabaseUserService
    {
        SQLiteAsyncConnection Database;

        async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            await Database.CreateTableAsync<User>();
        }

        public async Task AddUser(User user)
        {
            await Init();

            await Database.InsertAsync(user);

        }




        public async Task<User> GetUserByUsername(string username)
        {
            await Init();

            return await Database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            await Init();

            return await Database.Table<User>().FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task UpdateUser(User user)
        {
            await Init();

            await Database.UpdateAsync(user);

        }


        public async Task<bool> CheckExistingUserByEmail(string email)
        {
            await Init();

            var user = await Database.Table<User>().FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return false;
            else
                return true;
        }

        public async Task<bool> CheckExistingUserByUsername(string username)
        {
            await Init();

            var user = await Database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return false;
            else
                return true;
        }
    }
}

