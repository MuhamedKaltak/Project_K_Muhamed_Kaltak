using CommunityToolkit.Mvvm.Input;
using Project_K.Services;
using Project_K.Utilities;
using Project_K.View;
using Project_K.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Project_K.ViewModel
{
    [QueryProperty("User", "User")]
    public partial class ResetPasswordViewModel : BaseViewModel
    {
        DatabaseUserService databaseUserService;
        EmailService emailService;
        SecurityService securityService;

        [ObservableProperty]
        User user;

        public string email {  get; set; }
        public string token {  get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }

        public ResetPasswordViewModel(DatabaseUserService databaseUserService,SecurityService securityService, EmailService emailService)
        {
            Title = "Reset Password";
            this.databaseUserService = databaseUserService;
            this.securityService = securityService;
            this.emailService = emailService;
        }

        [RelayCommand]
        public async Task NavigateToLoginPage()
        {
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
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

                User = await databaseUserService.GetUserByEmail(email);

                if(User == null)
                {
                    await UINotification.DisplayAlertMessage("ERROR", "Email does not exist in the system", "OK");
                    return;
                }

                var token = await securityService.GenerateToken(User);

                User.PasswordResetToken = await securityService.Hash(token,User.Salt);
                User.PasswordResetDate = DateTime.Now;

                await databaseUserService.UpdateUser(User);

                await emailService.SendEmail(User.Email, "Reset Password", $"Your password reset token : {token}. The validity of the token expires in five minutes");
                await UINotification.DisplayAlertMessage("Email Sent", $"Email with a token has been sent to {  User.Email}", "OK");

                await NavigateToResetPasswordTokenPage(User);

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

                TimeSpan timeSpan = DateTime.Now - User.PasswordResetDate;

                if (timeSpan.Minutes <=5)
                {
                    User.Password = await securityService.Hash(password, User.Salt);
                    await UINotification.DisplayAlertMessage("Passwords have been changed", $"The password for your account has been successfully changed", "OK");
                    await NavigateToLoginPage();
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