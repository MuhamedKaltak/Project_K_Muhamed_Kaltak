using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DatabaseAccess.Model;
using Microsoft.Maui.ApplicationModel.Communication;
using Project_K.Messages;
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
    public partial class UserProfileViewModel : BaseViewModel, IRecipient<UpdateUserItemMessage>
    {
        DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework;
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

        public UserProfileViewModel(DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework, UserService userService, SecurityService securityService, EmailService emailService, IMediaPicker mediaPicker)
        {
            Title = "UserProfile";
            this.databaseUserServiceEntityFramework = databaseUserServiceEntityFramework;
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

            WeakReferenceMessenger.Default.Register<UpdateUserItemMessage>(this); //Subbat till messagen
        }

        [RelayCommand]
        async Task ChangeProfilePicture()
        {
            var file = await mediaPicker.PickPhotoAsync();

            if (file == null) return;

            picture = ImageTool.CompressAndResizeImage(file.FullPath, 200, 200, 80);

            User.ProfilePicture = picture;

            await databaseUserServiceEntityFramework.UpdateUser(User);

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

            if (await databaseUserServiceEntityFramework.CheckExistingUserByUsername(User.Username))
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

        async Task NavigateToChangeEmailPage()
        {
            await Shell.Current.GoToAsync($"{nameof(ChangeEmailPage)}", true);
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

                await databaseUserServiceEntityFramework.UpdateUser(User);

                await UINotification.DisplayAlertMessage("Email Sent", $"Email with a token will be sent to {User.Email}", "OK");

                await NavigateToChangeEmailPage();

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

                await databaseUserServiceEntityFramework.UpdateUser(User);

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

        public void Receive(UpdateUserItemMessage message)
        {
            OnPropertyChanged(nameof(User));

            // I'm using a pub/sub system because it's not sufficient for the OnPropertyChanged event to trigger when the Users value changes in, for example, - 
            // - ChangeEmailViewModel (which refers to UserService.user) where the email gets changed the OnPropertyChanged is not fired automatically as
            // - it needs to be explicitly called for data binding changes to be reflected.
        }
    }
}
