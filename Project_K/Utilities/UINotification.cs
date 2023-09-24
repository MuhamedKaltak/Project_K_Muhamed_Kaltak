using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Utilities
{
    public static class UINotification
    {
        public static async Task<bool> CheckValidField(List<string> fields)
        {

            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field))
                {
                    await ErrorMessageEmpty();
                    return false;
                }
                else if (field.Contains(" "))
                {
                    await ErrorMessageWhitspace();
                    return false;
                }
            }

            return true;
        }


        public static async Task DisplayAlertMessage(string title, string message, string cancel)
        {
            await Shell.Current.DisplayAlert(title, message, cancel);
        }

        private static async Task ErrorMessageEmpty()
        {
            await Shell.Current.DisplayAlert("ERROR", "Fields cannot be empty", "OK");
        }

        private static async Task ErrorMessageWhitspace()
        {
            await Shell.Current.DisplayAlert("ERROR", "Fields cannot contain whitespaces", "OK");
        }
    }
}
