using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_K.DataAccess;
using Project_K.Services;
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
        RegisterService registerService;


        IMediaPicker mediaPicker;


        [ObservableProperty]
        ImageSource imageToShowSource;


        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }

        public RegisterViewModel(RegisterService registerService ,IMediaPicker mediaPicker)
        {
            this.registerService = registerService;
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

            var byteArray = File.ReadAllBytes(file.FullPath); //Detta ska sparas till databas framöver

            ImageToShowSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
        }

        [RelayCommand]
        async Task RegisterNewUser()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await registerService.RegisterUserToDatabase(username,password,name,lastName,email);

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
