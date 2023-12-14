using CommunityToolkit.Mvvm.Input;
using Project_K.Services;
using Project_K.Utilities;
using Project_K.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework;
        SecurityService securityService;
        UserService userService;

        public string username {  get; set; }
        public string password { get; set; }

        private const string ADMIN_USERNAME_PASSWORD = "admin";

        public LoginViewModel(DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework, SecurityService securityService,UserService userService)
        {
            this.databaseUserServiceEntityFramework = databaseUserServiceEntityFramework;
            this.securityService = securityService;
            this.userService = userService;
        }


        [RelayCommand]
        async Task NavigateToRegisterPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(RegisterPage)}");
        }

        [RelayCommand]
        async Task NavigateToRecoverUsernamePageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(RecoverUsernamePage)}");
        }

        [RelayCommand]
        async Task NavigateToResetPasswordEmailPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(ResetPasswordEmailPage)}");
        }


        [RelayCommand]
        async Task Login()
        {

            if (IsBusy || !await UINotification.CheckValidField(new List<string> { username,password}))
                return;

            try
            {
                IsBusy = true;

                var user = await databaseUserServiceEntityFramework.GetUserByUsername(username);

                if (user == null)
                {
                    await Shell.Current.DisplayAlert("ERROR", "Could not login, please check your username and or password", "OK");
                    return;
                }

                //if (username == ADMIN_USERNAME_PASSWORD && password == ADMIN_USERNAME_PASSWORD) //Primarly used to avoid slow emulator hashing speed
                //{
                //    userService.user = user;

                //    Application.Current.MainPage = new AppShellMain();
                //    //await Shell.Current.GoToAsync("//Home");

                //    return;
                //}

                if (await securityService.VerifyPassword(user, password))
                {
                    userService.user = user;

                    Application.Current.MainPage = new AppShellMain();
                    //await Shell.Current.GoToAsync("//Home");
                }
                else
                {
                    await Shell.Current.DisplayAlert("ERROR", "Could not login, please check your username and or password", "OK");
                    return;
                }

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"ERROR:  {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
