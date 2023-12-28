using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DatabaseAccess.Model;
using Project_K.Messages;
using Project_K.Services;
using Project_K.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class ChangeEmailViewModel : BaseViewModel
    {
        DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework;
        SecurityService securityService;
        UserService userService;
        EmailService emailService;

        User user;

        public bool EnterTokenForCurrentEmail { get; set; }
        public bool EnterNewEmail { get; set; }
        public bool EnterTokenForNewEmail { get; set; }

        [ObservableProperty]
        public string icon;

        [ObservableProperty]
        public string placeholder;

        [ObservableProperty]
        public string input;

        [ObservableProperty]
        public string buttonText;

        private string newEmail;

        public ChangeEmailViewModel(DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework, SecurityService securityService, EmailService emailService, UserService userService)
        {
            this.databaseUserServiceEntityFramework = databaseUserServiceEntityFramework;
            this.securityService = securityService;
            this.emailService = emailService;
            this.userService = userService;

            user = userService.user;

            EnterTokenForCurrentEmail = true;
            placeholder = "Enter the token that was sent to your email";
            icon = FontAwesomeIcons.Code;
            ButtonText = "Verify Token";
        }

        [RelayCommand]
        public async Task ChangeEmail()
        {
            switch (true)
            {
                case var _ when EnterTokenForCurrentEmail: // I C# 7.0+ så kan man göra switch cases med bool values så här
                    await VerifyCurrentEmailToken();
                    break;

                case var _ when EnterNewEmail:
                    await VerifyNewEmail();
                    break;

                case var _ when EnterTokenForNewEmail:
                    await VerifyNewEmailToken();
                    break;

                default:
                    await UINotification.DisplayAlertMessage("ERROR", "Something went wrong", "OK");
                    await NavigateBackToPreviousPage();
                    break;
            }
        }

        private void NextState(string newPlaceholderValue)
        {
            Input = "";
            Placeholder = newPlaceholderValue;

            if (EnterTokenForCurrentEmail)
            {
                EnterTokenForCurrentEmail = false;
                EnterNewEmail = true;

                Icon = FontAwesomeIcons.Envelope;

                ButtonText = "Verify email";
            }
            else if (EnterNewEmail)
            {
                EnterNewEmail = false;
                EnterTokenForNewEmail = true;

                Icon = FontAwesomeIcons.Code;

                ButtonText = "Verify Token";
            }

        }

        async Task<bool> HasTokenExpired(TimeSpan timeSpan)
        {
            if (timeSpan.Minutes >= 5)
            {
                user.ResetToken = "";
                user.ResetDate = DateTime.MinValue;

                await databaseUserServiceEntityFramework.UpdateUser(user);
                await UINotification.DisplayAlertMessage("ERROR", $"ERROR: Token has expired, navigating back to profile page", "OK");

                return true;
            }

            return false;
        }

        async Task VerifyCurrentEmailToken()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { Input }))
                return;

            try
            {
                IsBusy = true;

                TimeSpan timeSpan = DateTime.Now - user.ResetDate;

                
                if (await HasTokenExpired(timeSpan))
                {
                    await NavigateBackToPreviousPage();
                }
                else if (await securityService.VerifyToken(user, Input))
                {
                    NextState("Enter your new desired email address");
                }
                else
                {
                    await UINotification.DisplayAlertMessage("Invalid token", $"The token provided is not valid", "OK");
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

        async Task VerifyNewEmail()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { Input }))
                return;

            if (!await emailService.CheckEmailFormat(Input))
            {
                await UINotification.DisplayAlertMessage("ERROR", "The email provided is not in a correct format. Example on correct format -> (abc@abc.se)", "OK");
                return;
            }

            if (await databaseUserServiceEntityFramework.CheckExistingUserByEmail(Input))
            {
                await Shell.Current.DisplayAlert("ERROR", "The email provied already exists in the system", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var token = await securityService.GenerateToken();
                user.ResetToken = await securityService.Hash(token, user.Salt);

                await emailService.SendEmail(Input, "Change Email", $"Your email reset token : {token}. The validity of the token expires in five minutes");

                user.ResetDate = DateTime.Now;

                await databaseUserServiceEntityFramework.UpdateUser(user);

                newEmail = Input;

                NextState("Enter the token that was sent to the new email address");
                
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

        async Task VerifyNewEmailToken()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { Input }))
                return;

            try
            {
                IsBusy = true;

                TimeSpan timeSpan = DateTime.Now - user.ResetDate;

                if (await HasTokenExpired(timeSpan))
                {
                    await NavigateBackToPreviousPage();
                }
                else if (await securityService.VerifyToken(user, Input))
                {
                    user.ResetToken = "";
                    user.ResetDate = DateTime.MinValue;

                    var oldEmail = user.Email;

                    user.Email = newEmail;

                    IsBusy = false;

                    await databaseUserServiceEntityFramework.UpdateUser(user);

                    WeakReferenceMessenger.Default.Send(new UpdateUserItemMessage("Email has been updated"));

                    await NavigateBackToPreviousPage();

                    await UINotification.DisplayAlertMessage("Email has been changed", "Your email has now been changed", "OK");

                    await emailService.SendEmail(oldEmail, "Email has been changed", "The email associated with your account in Project_K has been changed, if this was not you then contact support immediately.");
                }
                else
                {
                    await UINotification.DisplayAlertMessage("Invalid token", $"The token provided is not valid", "OK");

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
    }
}
