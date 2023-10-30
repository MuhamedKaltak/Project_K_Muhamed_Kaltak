using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_K.Model;
using Project_K.Services;
using Project_K.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class UserProfileViewModel : BaseViewModel
    {
        DatabaseUserService databaseUserService;
        UserService userService;

        IMediaPicker mediaPicker;

        [ObservableProperty]
        ImageSource profileImage;

        [ObservableProperty]
        User user;

        private User originalUserData;

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

        public UserProfileViewModel(DatabaseUserService databaseUserService, UserService userService, IMediaPicker mediaPicker)
        {
            this.databaseUserService = databaseUserService;
            this.userService = userService;
            this.mediaPicker = mediaPicker;

            User = userService.user; //Kopierar referensen av objektet, skapar ej en ny kopia av objektet. Typ som pekare i C++ när det gäller asignment mellan objekt i C#. -
            originalUserData = DeepCopy(User); //- Därför gör man en DeepCopy för att få en kopia av datan från ett annant objekt men ej referensen.

            profileImage = ImageSource.FromStream(() => new MemoryStream(user.ProfilePicture));
        }

        [RelayCommand]
        async Task ChangeProfilePicture()
        {
            var file = await mediaPicker.PickPhotoAsync();

            if (file == null) return;

            picture = ImageTool.CompressAndResizeImage(file.FullPath, 200, 200, 80);

            userService.user.ProfilePicture = picture;

            await databaseUserService.UpdateUser(userService.user);

            ProfileImage = ImageSource.FromStream(() => new MemoryStream(picture));
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
        async Task<bool> UpdateUserDetails()
        {
            if (IsBusy || !await UINotification.CheckValidField(new List<string> { User.Username }))
                return false;

            try
            {
                IsBusy = true;

                await databaseUserService.UpdateUser(User);

                await UINotification.DisplayAlertMessage("Updated Data", $"Succesfully updated your account details", "OK");

                await Task.Run(() =>
                {
                    originalUserData = DeepCopy(User);
                });

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

    }
}
