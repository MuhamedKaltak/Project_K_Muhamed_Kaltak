using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_K.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class RegisterViewModel : BaseViewModel
    {
        IMediaPicker mediaPicker;


        [ObservableProperty]
        ImageSource imageToShowSource;

        public RegisterViewModel(IMediaPicker mediaPicker)
        {
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
    }
}
