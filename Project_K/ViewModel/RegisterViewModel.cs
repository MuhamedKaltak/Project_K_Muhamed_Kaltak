using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using DatabaseAccess.Model;
using Project_K.Model;
using Project_K.Services;
using Project_K.Utilities;
using Project_K.View;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class RegisterViewModel : BaseViewModel
    {
        DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework;
        SecurityService securityService;
        EmailService emailService;


        IMediaPicker mediaPicker;


        [ObservableProperty]
        ImageSource imageToShowSource;


        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        private byte[] profilePicture { get; set; }


        public RegisterViewModel(DatabaseUserServiceEntityFramework databaseUserServiceEntityFramework, SecurityService securityService, EmailService emailService ,IMediaPicker mediaPicker)
        {
            Title = "Register a new account";
            this.databaseUserServiceEntityFramework = databaseUserServiceEntityFramework;
            this.securityService = securityService;
            this.emailService = emailService;
            this.mediaPicker = mediaPicker;
            imageToShowSource = "user.png";
        }

        [RelayCommand]
        async Task PickProfilePicture()
        {
            var file = await mediaPicker.PickPhotoAsync();

            if (file == null) return;

            profilePicture = ImageTool.CompressAndResizeImage(file.FullPath, 200, 200, 80);

            ImageToShowSource = ImageSource.FromStream(() => new MemoryStream(profilePicture));
        }

        [RelayCommand]
        async Task RegisterNewUser()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { username, password, confirmPassword, name, lastName, email }) || !await ArePasswordsMatching())
                return;

            if (!await emailService.CheckEmailFormat(email))
            {
                await UINotification.DisplayAlertMessage("ERROR", "The email provided is in an incorrect format, should be in this format -> (abc.def.se)", "OK");
                return;
            }

            if (await databaseUserServiceEntityFramework.CheckExistingUserByEmail(email))
            {
                await Shell.Current.DisplayAlert("ERROR", "The email provied already exists in the system", "OK");
                return;
            }
            else if (await databaseUserServiceEntityFramework.CheckExistingUserByUsername(username))
            {
                await Shell.Current.DisplayAlert("ERROR", "The username provied already exists in the system, please choose a different username", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var salt = await securityService.GenerateRandomSalt(16);
                var hashedPassword = await securityService.Hash(password, salt);

                DatabaseAccess.Model.User user = new DatabaseAccess.Model.User
                {
                    Username = username,
                    Password = hashedPassword,
                    Name = name,
                    LastName = lastName,
                    Email = email,
                    Salt = salt,
                    ProfilePicture = profilePicture
                };

                await databaseUserServiceEntityFramework.AddUser(user);

                await NavigateBackToPreviousPage();

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"ERROR:  {ex.Message}", "OK");
            }
            finally { IsBusy = false; }
        }

        private async Task<bool> ArePasswordsMatching()
        {
            if (password.Equals(confirmPassword))
                return true;

            await UINotification.DisplayAlertMessage("ERROR", "The passwords do not match", "OK");

            return false;
        }
    }
}
