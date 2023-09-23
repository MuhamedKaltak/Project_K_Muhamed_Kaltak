using Project_K.DataAccess;
using Project_K.Model;
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

        public async Task<bool> CheckValidFields(string username, string password, string confirmPassword, string name, string lastName, string email)
        {
            if (string.IsNullOrEmpty(username))
            {
                await ErrorMessageEmpty("Username");
                return false;
            }
            else if (username.Contains(" "))
            {
                await ErrorMessageWhitspace("Username");
                return false;
            }
            else if (string.IsNullOrEmpty(password))
            {
                await ErrorMessageEmpty("Password");
                return false;
            }
            else if (password.Contains(" "))
            {
                await ErrorMessageWhitspace("Password");
                return false;
            }
            else if (!password.Equals(confirmPassword))
            {
                await Shell.Current.DisplayAlert("Error!", "The passwords do not match", "OK");
                return false;
            }
            else if (string.IsNullOrEmpty(name))
            {
                await ErrorMessageEmpty("Name");
                return false;
            }
            else if (name.Contains(" "))
            {
                await ErrorMessageWhitspace("Name");
                return false;
            }
            else if (string.IsNullOrEmpty(lastName))
            {
                await ErrorMessageEmpty("Last name");
                return false;
            }
            else if (lastName.Contains(" "))
            {
                await ErrorMessageWhitspace("Last name");
                return false;
            }
            else if (string.IsNullOrEmpty(email))
            {
                await ErrorMessageEmpty("Email");
                return false;
            }
            else if (email.Contains(" "))
            {
                await ErrorMessageWhitspace("Email");
                return false;
            }
            else if (!await IsEmailInValidFormat(email))
            {
                await Shell.Current.DisplayAlert("Error!", "The email field is not in a valid format. Should be in this format -> 'abc@abc.se' ", "OK");
                return false;
            }

            return true;
        }

        public async Task<bool> EmailAlreadyInUse(string email)
        {
            var emailInUse = await DatabaseManager.CheckExistingUser(email);

            if (emailInUse)
            {
                await Shell.Current.DisplayAlert("Error!", "Could not create the account", "OK");
                return true;
            }

            return false;
        }

        private async Task ErrorMessageEmpty(string field)
        {
            await Shell.Current.DisplayAlert("Error!", $"{field} field cannot be empty", "OK");
        }

        private async Task ErrorMessageWhitspace(string field)
        {
            await Shell.Current.DisplayAlert("Error!", $"{field} field cannot contain whitespaces", "OK");
        }

        private async Task<bool> IsEmailInValidFormat(string email)
        {
            return await Task.Run(() =>
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            });
        }
    }
}
