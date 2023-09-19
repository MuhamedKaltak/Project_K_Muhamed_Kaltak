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
            await Database.CreateTableAsync<TEST>();
        }

        public static async Task AddUserTEST()
        {
            await Init();

            var user = new User
            {
                Username = "TestUsername",
                Password = "password",
                Name = "Test",
                LastName = "Test",
                Email = "Test"
            };

            await Database.InsertAsync(user);

            var test = new TEST
            {
                UsernameTEST = "TestUsername"
                
            };

            await Database.InsertAsync(test);
        }
    }
}
