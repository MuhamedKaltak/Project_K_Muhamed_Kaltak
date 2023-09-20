using Project_K.Model;
using Project_K.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.DataAccess
{
    public static class DatabaseManager
    {
        static SQLiteAsyncConnection Database;

        static async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            await Database.CreateTableAsync<User>();
        }

        public static async Task AddUser(string username, string password, string name, string lastName, string email)
        {
            await Init();

            var user = new User
            {
                Username = username,
                Password = password,
                Name = name,
                LastName = lastName,
                Email = email
            };

            await Database.InsertAsync(user);

        }
    }
}
