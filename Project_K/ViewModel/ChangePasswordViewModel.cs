using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess.Model;
using Project_K.Services;
using Project_K.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class ChangePasswordViewModel : BaseViewModel
    {
        DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework;
        SecurityService securityService;
        UserService userService;
        EmailService emailService;

        User user;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EnterNewPassword))]
        public bool enterCurrentPassword;
        public bool EnterNewPassword => !EnterCurrentPassword;

        [ObservableProperty]
        public string currentPassword;

        [ObservableProperty]
        public string newPassword;

        [ObservableProperty]
        public string confirmPassword;


        public ChangePasswordViewModel(DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework, SecurityService securityService, EmailService emailService, UserService userService)
        {
            this.databaseUserServiceEntityFramework = databaseUserServiceEntityFramework;
            this.securityService = securityService;
            this.emailService = emailService;
            this.userService = userService;

            user = userService.user;

            EnterCurrentPassword = true;
        }

        [RelayCommand]
        public async Task ChangePassword()
        {
            switch (true)
            {
                case var _ when EnterCurrentPassword:
                    await VerifyPassword();
                    break;

                case var _ when EnterNewPassword:
                    await SetNewPassword();
                    break;

                default:
                    await UINotification.DisplayAlertMessage("ERROR", "Something went wrong", "OK");
                    await NavigateBackToPreviousPage();
                    break;
            }
        }

        [RelayCommand]
        async Task VerifyPassword()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { CurrentPassword }))
                return;

            try
            {
                IsBusy = true;

                if (await securityService.VerifyPassword(user, CurrentPassword))
                {
                    CurrentPassword = "";
                    EnterCurrentPassword = false;
                }
                else
                {
                    await Shell.Current.DisplayAlert("ERROR", "Incorrect password, please try again", "OK");
                    return;
                }

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

        [RelayCommand]
        async Task SetNewPassword()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { NewPassword, ConfirmPassword }))
                return;

            try
            {
                IsBusy = true;

                if (!NewPassword.Equals(ConfirmPassword))
                {
                    await UINotification.DisplayAlertMessage("ERROR", $"The passwords do not match, please try again", "OK");
                    return;
                }

                user.Password = await securityService.Hash(NewPassword, user.Salt);

                await databaseUserServiceEntityFramework.UpdateUser(user);

                await NavigateBackToPreviousPage();

                await UINotification.DisplayAlertMessage("Password has been changed", "Your password has now been changed", "OK");

                await emailService.SendEmail(user.Email, "Password has been changed", "The password associated with your account in Project_K has been changed, if this was not you then contact support immediately.");

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
