using Project_K.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Services
{
    public class RegisterService
    {
        public async Task RegisterUserToDatabase(string username, string password, string name, string lastName, string email)
        {
            await DatabaseManager.AddUser(username, password, name, lastName, email);
        }
    }
}
