using CommunityToolkit.Mvvm.Input;
using Project_K.Services;
using Project_K.Utilities;
using Project_K.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DatabaseAccess.Model;

namespace Project_K.ViewModel
{
    [QueryProperty("User", "User")]
    public partial class ResetPasswordViewModel : BaseViewModel
    {
        DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework;
        EmailService emailService;
        SecurityService securityService;

        [ObservableProperty]
        User user;

        public string email {  get; set; }
        public string token {  get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }

        public ResetPasswordViewModel(DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework, SecurityService securityService, EmailService emailService)
        {
            Title = "Reset Password";
            this.databaseUserServiceEntityFramework = databaseUserServiceEntityFramework;
            this.securityService = securityService;
            this.emailService = emailService;
        }

        [RelayCommand]
        public async Task NavigateToLoginPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        [RelayCommand]
        public async Task NavigateToResetPasswordTokenPage(User user)
        {
            await Shell.Current.GoToAsync($"{nameof(ResetPasswordTokenPage)}", true,
            new Dictionary<string, object>
            {
                {"User",user}
            });
        }

        [RelayCommand]
        public async Task NavigateToResetPasswordPage(User user)
        {
            await Shell.Current.GoToAsync($"{nameof(ResetPasswordPage)}", true,
            new Dictionary<string, object>
            {
                {"User",user}
            });
        }


        [RelayCommand]
        public async Task SendTokenToEmail()
        {
            if(IsBusy || !await UINotification.CheckValidField(new List<string> { email})) 
                return;

            try
            {
                IsBusy = true;

                User = await databaseUserServiceEntityFramework.GetUserByEmail(email);

                if(User == null)
                {
                    await UINotification.DisplayAlertMessage("ERROR", "Email does not exist in the system", "OK");
                    return;
                }

                await UINotification.DisplayAlertMessage("Email Sent", $"Email with a token will be sent to {User.Email}", "OK");

                await NavigateToResetPasswordTokenPage(User);

                var token = await securityService.GenerateToken();

                User.ResetToken = await securityService.Hash(token,User.Salt);
                User.ResetDate = DateTime.Now;

                await databaseUserServiceEntityFramework.UpdateUser(User);

                await emailService.SendEmail(User.Email, "Reset Password", $"Your password reset token : {token}. The validity of the token expires in five minutes");

            }
            catch (Exception ex)
            {
                await UINotification.DisplayAlertMessage("ERROR", $"ERROR:  {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        public async Task VerifyToken()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { token }))
                return;

            try
            {
                IsBusy = true;

                if (await securityService.VerifyToken(User,token))
                {
                    await NavigateToResetPasswordPage(User);
                }
                else
                {
                    await UINotification.DisplayAlertMessage("ERROR", $"Not a valid token, please try again", "OK");
                }

            }
            catch (Exception ex)
            {
                await UINotification.DisplayAlertMessage("ERROR", $"ERROR:  {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ResetPassword()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { password,confirmPassword }))
                return;

            if (!password.Equals(confirmPassword))
            {
                await UINotification.DisplayAlertMessage("ERROR", $"The passwords do not match, please try again", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                TimeSpan timeSpan = DateTime.Now - User.ResetDate;

                if (timeSpan.Minutes <=5)
                {
                    User.Password = await securityService.Hash(password, User.Salt);

                    await databaseUserServiceEntityFramework.UpdateUser(User);
                    await UINotification.DisplayAlertMessage("Passwords have been changed", $"The password for your account has been successfully changed", "OK");
                    await NavigateToLoginPage();
                    await emailService.SendEmail(User.Email, "Passwords have been changed", "The password associated with your account in Project_K has been changed, if this was not you contact support immediately.");

                }
                else
                {
                    await UINotification.DisplayAlertMessage("ERROR", $"ERROR: Token has expired", "OK");
                    await NavigateToLoginPage();
                }
                

            }
            catch (Exception ex)
            {
                await UINotification.DisplayAlertMessage("ERROR", $"ERROR:  {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}