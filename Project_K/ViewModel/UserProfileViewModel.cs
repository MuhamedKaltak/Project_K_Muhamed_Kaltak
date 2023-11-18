using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel.Communication;
using Project_K.Model;
using Project_K.Services;
using Project_K.Utilities;
using Project_K.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class UserProfileViewModel : BaseViewModel
    {
        DatabaseUserService databaseUserService;
        UserService userService;
        SecurityService securityService;
        EmailService emailService;

        IMediaPicker mediaPicker;

        [ObservableProperty]
        ImageSource profileImage;

        [ObservableProperty]
        User user;

        private User originalUserData;

        public string enteredPassword {  get; set; }
        public string newPassword {  get; set; }
        public string confirmNewPassword {  get; set; }
        public string enteredEmail { get; set; }
        public string enteredToken { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditingUsername))]
        bool editUsername;

        public bool EditingUsername => !EditUsername;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditingName))]
        bool editName;

        public bool EditingName => !EditName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EditingLastName))]
        bool editLastName;

        public bool EditingLastName => !EditLastName;

        [ObservableProperty]
        bool editEmail;

        private byte[] picture { get; set; }

        public UserProfileViewModel(DatabaseUserService databaseUserService, UserService userService, SecurityService securityService, EmailService emailService, IMediaPicker mediaPicker)
        {
            Title = "UserProfile";
            this.databaseUserService = databaseUserService;
            this.userService = userService;
            this.securityService = securityService;
            this.mediaPicker = mediaPicker;
            this.emailService = emailService;

            User = userService.user; //Kopierar referensen av objektet, skapar ej en ny kopia av objektet. Typ som pekare i C++ när det gäller asignment mellan objekt i C#. -
            originalUserData = DeepCopy(User); //- Därför gör man en DeepCopy för att få en kopia av datan från ett annant objekt men ej referensen.


            if (User.ProfilePicture == null)
            {
                ImageToShowTabBar = "user.png";
            }
            else
            {
                ImageToShowTabBar = ImageSource.FromStream(() => new MemoryStream(User.ProfilePicture));
            }
        }

        [RelayCommand]
        async Task ChangeProfilePicture()
        {
            var file = await mediaPicker.PickPhotoAsync();

            if (file == null) return;

            picture = ImageTool.CompressAndResizeImage(file.FullPath, 200, 200, 80);

            User.ProfilePicture = picture;

            await databaseUserService.UpdateUser(User);

            ImageToShowTabBar = ImageSource.FromStream(() => new MemoryStream(picture));

        }

        [RelayCommand]
        private void ModifyUsername()
        {
            EditUsername = true;
        }

        [RelayCommand]
        async Task ApplyModificationsToUsername()
        {
            if (User.Username == originalUserData.Username)
            {
                EditUsername = false;
                return;
            }

            if (!await UINotification.CheckValidField(new List<string> { User.LastName }))
                return;

            if (await databaseUserService.CheckExistingUserByUsername(User.Username))
            {
                await Shell.Current.DisplayAlert("ERROR", "The username provied already exists in the system, please choose a different username", "OK");
                return;
            }

            if (await UpdateUserDetails())
                EditUsername = false;
            
        }

        [RelayCommand]
        private void CancelModificationsToUsername()
        {
            EditUsername = false;
            User.Username = originalUserData.Username;

            OnPropertyChanged(nameof(User));
        }

        [RelayCommand]
        private void ModifyName()
        {
            EditName = true;
        }

        [RelayCommand]
        async Task ApplyModificationsToName()
        {
            if (User.Name == originalUserData.Name)
            {
                EditName = false;
                return;
            }

            if (!await UINotification.CheckValidField(new List<string> { User.Name }))
                return;

            if (await UpdateUserDetails())
                EditName = false;
        }

        [RelayCommand]
        private void CancelModificationsToName()
        {
            EditName = false;
            User.Name = originalUserData.Name;

            OnPropertyChanged(nameof(User));
        }

        [RelayCommand]
        private void ModifyLastName()
        {
            EditLastName = true;
        }

        [RelayCommand]
        async Task ApplyModificationsToLastName()
        {
            if (User.LastName == originalUserData.LastName)
            {
                EditLastName = false;
                return;
            }

            if (!await UINotification.CheckValidField(new List<string> { User.LastName }))
                return;
            

            if (await UpdateUserDetails())
                EditLastName = false;
        }

        [RelayCommand]
        private void CancelModificationsToLastName()
        {
            EditLastName = false;
            User.LastName = originalUserData.LastName;

            OnPropertyChanged(nameof(User));
        }

        [RelayCommand]
        async Task NavigateToChangePasswordSecurityCheckPage()
        {
            await Shell.Current.GoToAsync($"{nameof(ChangePasswordSecurityCheckPage)}",true);
        }

        [RelayCommand]
        async Task NavigateToUserProfilePage()
        {
            await Shell.Current.GoToAsync($"{nameof(UserProfilePage)}",true);
        }

        async Task NavigateToChangePasswordPage()
        {
            await Shell.Current.GoToAsync($"{nameof(ChangePasswordPage)}",true);
        }

        async Task NavigateToChangeEmailCurrentEmailTokenPage()
        {
            await Shell.Current.GoToAsync($"{nameof(ChangeEmailCurrentEmailTokenPage)}",true);
        }

        async Task NavigateToChangeEmailNewEmailPage()
        {
            await Shell.Current.GoToAsync($"{nameof(ChangeEmailNewEmailPage)}", true);
        }

        async Task NavigateToChangeEmailNewEmailTokenPage()
        {
            await Shell.Current.GoToAsync($"{nameof(ChangeEmailNewEmailTokenPage)}", true);
        }

        [RelayCommand]
        async Task PasswordSecurityCheck()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { enteredPassword }))
                return;

            try
            {
                IsBusy = true;

                if (await securityService.VerifyPassword(User, enteredPassword))
                {
                    enteredPassword = "";
                    await NavigateToChangePasswordPage();
                }
                else
                {
                    await Shell.Current.DisplayAlert("ERROR", "Incorrect password, please try again", "OK");
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

        [RelayCommand]
        async Task ChangePassword()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { newPassword, confirmNewPassword }))
                return;

            try
            {
                IsBusy = true;

                if (!newPassword.Equals(confirmNewPassword))
                {
                    await UINotification.DisplayAlertMessage("ERROR", $"The passwords do not match, please try again", "OK");
                    return;
                }

                User.Password = await securityService.Hash(newPassword, User.Salt);
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

            if(await UpdateUserDetails())
            {
                await NavigateToUserProfilePage();
                await emailService.SendEmail(User.Email, "Passwords have been changed", "The password associated with your account in Project_K has been changed, if this was not you then contact support immediately.");
            }

        }

        [RelayCommand]
        async Task SendTokenToCurrentEmail()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var token = await securityService.GenerateToken();

                User.ResetToken = await securityService.Hash(token, User.Salt);
                User.ResetDate = DateTime.Now;

                await databaseUserService.UpdateUser(User);

                await UINotification.DisplayAlertMessage("Email Sent", $"Email with a token will be sent to {User.Email}", "OK");

                await NavigateToChangeEmailCurrentEmailTokenPage();

                IsBusy = false;

                await emailService.SendEmail(User.Email, "Change Email", $"Your email reset token : {token}. The validity of the token expires in five minutes");


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

        [RelayCommand]
        async Task VerifyCurrentEmailToken()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { enteredToken }))
                return;

            try
            {
                IsBusy = true;

                TimeSpan timeSpan = DateTime.Now - User.ResetDate;

                if (timeSpan.Minutes >= 5)
                {
                    User.ResetToken = "";

                    await databaseUserService.UpdateUser(User);
                    await UINotification.DisplayAlertMessage("ERROR", $"ERROR: Token has expired, navigating back to profile page", "OK");
                    await NavigateToUserProfilePage();
                }

                if (await securityService.VerifyToken(User,enteredToken))
                {
                    enteredToken = "";
                    await NavigateToChangeEmailNewEmailPage();

                }
                else
                {
                    await UINotification.DisplayAlertMessage("Invalid token", $"The token provided is not valid", "OK");
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

        [RelayCommand]
        async Task VerifyNewEmail()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { enteredEmail }))
                return;

            if (!await emailService.CheckEmailFormat(enteredEmail))
            {
                await UINotification.DisplayAlertMessage("ERROR", "The email provided is not in a correct format. Example on correct format -> (abc@abc.se)", "OK");
                return;
            }

            if (await databaseUserService.CheckExistingUserByEmail(enteredEmail))
            {
                await Shell.Current.DisplayAlert("ERROR", "The email provied already exists in the system", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var token = await securityService.GenerateToken();
                User.ResetToken = await securityService.Hash(token, User.Salt);

                await emailService.SendEmail(enteredEmail, "Change Email", $"Your email reset token : {token}. The validity of the token expires in five minutes");

                User.ResetDate = DateTime.Now;

                await databaseUserService.UpdateUser(User);

                await NavigateToChangeEmailNewEmailTokenPage();
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

        [RelayCommand]
        async Task VerifyNewEmailToken()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { enteredToken }))
                return;

            try
            {
                IsBusy = true;

                TimeSpan timeSpan = DateTime.Now - User.ResetDate;

                if (timeSpan.Minutes >= 5)
                {
                    User.ResetToken = "";

                    await databaseUserService.UpdateUser(User);
                    await UINotification.DisplayAlertMessage("ERROR", $"ERROR: Token has expired, navigating back to profile page", "OK");
                    await NavigateToUserProfilePage();
                }

                if (await securityService.VerifyToken(User, enteredToken))
                {
                    enteredToken = "";
                    User.ResetToken = "";

                    User.Email = enteredEmail;

                    var oldEmail = originalUserData.Email;

                    IsBusy = false;

                    await UpdateUserDetails();

                    await NavigateToUserProfilePage();

                    await emailService.SendEmail(oldEmail, "Email has been changed", "The email associated with your account in Project_K has been changed, if this was not you then contact support immediately.");
                }
                else
                {
                    await UINotification.DisplayAlertMessage("Invalid token", $"The token provided is not valid", "OK");

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

        [RelayCommand]
        async Task Logout()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                userService.user = null;

                Application.Current.MainPage = new AppShellLogin();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"ERROR:  {ex.Message}", "OK");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        async Task<bool> UpdateUserDetails()
        {
            if (IsBusy)
                return false;

            try
            {
                IsBusy = true;

                await databaseUserService.UpdateUser(User);

                await UINotification.DisplayAlertMessage("Updated Data", $"Succesfully updated your account details", "OK");

                originalUserData = DeepCopy(User);
                
                OnPropertyChanged(nameof(User));

                return true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"ERROR:  {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
            
        }

        private User DeepCopy(User source)
        {
            var json = JsonSerializer.Serialize(source);
            return JsonSerializer.Deserialize<User>(json);
        }

        [RelayCommand]
        public void RefreshImage() //Workaround for .NET MAUI BUG, images not showing when changing ImageSource to another picture and reappearing to the same tab
        {
            TabBarRefreshImage_WorkAround();
        }

    }
}
