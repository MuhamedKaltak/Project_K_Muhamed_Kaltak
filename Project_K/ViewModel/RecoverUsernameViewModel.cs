using CommunityToolkit.Mvvm.Input;
using DatabaseAccess.Model;
using Project_K.Model;
using Project_K.Services;
using Project_K.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class RecoverUsernameViewModel : BaseViewModel
    {
        DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework;
        EmailService emailService;

        public string email {  get; set; }

        public RecoverUsernameViewModel(DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework, EmailService emailService)
        {
            Title = "Username Recovery";
            this.databaseUserServiceEntityFramework = databaseUserServiceEntityFramework;
            this.emailService = emailService;
        }

        [RelayCommand]
        public async Task RecoverUsername()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { email }))
                return;

            User user = await databaseUserServiceEntityFramework.GetUserByEmail(email);

            if (user == null)
            {
                await UINotification.DisplayAlertMessage("ERROR", "Email does not exist in the database", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                await UINotification.DisplayAlertMessage("Email Sent", $"Email with your username details will be sent to {user.Email} ", "OK");

                await NavigateBackToPreviousPage();

                await emailService.SendEmail(user.Email, "Recovery of your username", $"Your username in Project_K is: {user.Username}");

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error!", $"ERROR:  {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
