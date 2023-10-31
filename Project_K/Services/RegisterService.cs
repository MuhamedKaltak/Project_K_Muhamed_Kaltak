using Microsoft.Maui.ApplicationModel.Communication;
using Project_K.Model;
using Project_K.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project_K.Services
{
    public class RegisterService
    {   
        public async Task<bool> ArePasswordsMatching(string password, string confirmPassword)
        {
            var matchingPasswords = await CheckPasswords(password, confirmPassword);

            if (matchingPasswords)
                return true;

            await UINotification.DisplayAlertMessage("ERROR", "The passwords do not match", "OK");

            return false;
        }
        private async Task<bool> CheckPasswords(string password,string confirmPassword)
        {
            return await Task.Run(() =>
            {
                return password.Equals(confirmPassword);
            });
        }

    }
}
