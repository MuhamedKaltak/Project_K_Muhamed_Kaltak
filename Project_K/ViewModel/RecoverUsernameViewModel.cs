using CommunityToolkit.Mvvm.Input;
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
        DatabaseUserService databaseUserService;
        EmailService emailService;

        public string email {  get; set; }

        public RecoverUsernameViewModel(DatabaseUserService databaseUserService,EmailService emailService)
        {
            Title = "Username Recovery";
            this.databaseUserService = databaseUserService;
            this.emailService = emailService;
        }

        [RelayCommand]
        public async Task NavigateToLoginPage()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task RecoverUsername()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { email }))
                return;

            User user = await databaseUserService.GetUserByEmail(email);

            if (user == null)
            {
                await UINotification.DisplayAlertMessage("ERROR", "Email does not exist in the database", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                await emailService.SendEmail(user.Email,"Recovery of your username",$"Your username in Project_K is: {user.Username}");

                await UINotification.DisplayAlertMessage("Email Sent", $"Email has been sent to {user.Email} ", "OK");
                await NavigateToLoginPage();
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
