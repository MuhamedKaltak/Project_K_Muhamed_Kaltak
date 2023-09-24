using Microsoft.Maui.ApplicationModel.Communication;
using Project_K.DataAccess;
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
        public async Task RegisterUserToDatabase(User user)
        {
            await DatabaseManager.AddUser(user);
        }

        public async Task<bool> EmailAlreadyInUse(string email)
        {
            var emailInUse = await DatabaseManager.CheckExistingUser(email);

            if (emailInUse)
            {
                await Shell.Current.DisplayAlert("ERROR", "The email provied already exists in the system", "OK");
                return true;
            }

            return false;
        }


        public async Task<bool> IsEmailInValidFormat(string email)
        {
            var validEmail = await CheckEmailFormat(email);

            if (validEmail)
            {
                return true;
            }

            await UINotification.DisplayAlertMessage("ERROR", "The email provided is not in a correct format. Example on correct format -> (abc@abc.se)", "OK");
            return false;
        }
        private async Task<bool> CheckEmailFormat(string email)
        {
            return await Task.Run(() =>
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            });
        }

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
