using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class RegisterViewModel : BaseViewModel
    {
        DatabaseUserService databaseUserService;
        RegisterService registerService;
        SecurityService securityService;


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


        public RegisterViewModel(DatabaseUserService databaseUserService,RegisterService registerService,SecurityService security ,IMediaPicker mediaPicker)
        {
            Title = "Register a new account";
            this.databaseUserService = databaseUserService;
            this.registerService = registerService;
            this.securityService = security;
            this.mediaPicker = mediaPicker;
            imageToShowSource = "user.png";
        }


        [RelayCommand]
        async Task NavigateToLoginPageAsync()
        {
            await Shell.Current.GoToAsync("..",true);
        }

        [RelayCommand]
        async Task PickProfilePicture()
        {
            var file = await mediaPicker.PickPhotoAsync();

            if (file == null) return;

            profilePicture = File.ReadAllBytes(file.FullPath); //Detta ska sparas till databasen framöver

            ImageToShowSource = ImageSource.FromStream(() => new MemoryStream(profilePicture));
        }

        [RelayCommand]
        async Task RegisterNewUser()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { username, password, name, lastName, email }) || !await registerService.ArePasswordsMatching(password, confirmPassword) || !await registerService.IsEmailInValidFormat(email))
                return;

            if(await databaseUserService.CheckExistingUserByEmail(email))
            {
                await Shell.Current.DisplayAlert("ERROR", "The email provied already exists in the system", "OK");
                return;
            }
            else if (await databaseUserService.CheckExistingUserByUsername(username))
            {
                await Shell.Current.DisplayAlert("ERROR", "The username provied already exists in the system, please choose a different username", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var salt = await securityService.GenerateRandomSalt(16);
                var hashedPassword = await securityService.Hash(password,salt);

                profilePicture = ImageTool.CompressAndResizeImage(profilePicture, 200, 200, 80);

                User user = new User
                {
                    Username = username,
                    Password = hashedPassword,
                    Name = name,
                    LastName = lastName,
                    Email = email,
                    Salt = salt,
                    ProfilePicture = profilePicture
                };

                await databaseUserService.AddUser(user);

                await NavigateToLoginPageAsync();

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", $"ERROR:  {ex.Message}", "OK");
            }
            finally { IsBusy = false; }
        }
    }
}
