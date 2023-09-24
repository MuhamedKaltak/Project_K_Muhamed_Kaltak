using Project_K.DataAccess;
using Project_K.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Services
{
    public class LoginService
    {
        public async Task<User> GetUserByUsername(string username)
        {
            return await DatabaseManager.GetUserByUsername(username);
        }

        
    }
}
