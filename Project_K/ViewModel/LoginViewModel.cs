﻿using CommunityToolkit.Mvvm.Input;
using Project_K.Model;
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
        DatabaseUserService databaseUserService;
        SecurityService securityService;

        public string username {  get; set; }
        public string password { get; set; }

        public LoginViewModel(DatabaseUserService databaseUserService,SecurityService securityService)
        {
            this.databaseUserService = databaseUserService;
            this.securityService = securityService;
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
        async Task Login()
        {

            if (IsBusy || !await UINotification.CheckValidField(new List<string> { username,password}))
                return;

            try
            {
                IsBusy = true;

                var user = await databaseUserService.GetUserByUsername(username);

                if (user == null)
                {
                    await Shell.Current.DisplayAlert("ERROR", "Could not login, please check your username and or password", "OK");
                    return;
                }

                bool passwordCorrect = await securityService.VerifyPassword(user, password);

                if (passwordCorrect)
                {
                    await Shell.Current.DisplayAlert("LOGGA IN", "LOGGA IN", "OK");
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
