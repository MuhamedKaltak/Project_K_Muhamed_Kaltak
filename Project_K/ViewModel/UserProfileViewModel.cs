using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project_K.Model;
using Project_K.Services;
using Project_K.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private byte[] picture { get; set; }

        public UserProfileViewModel(DatabaseUserService databaseUserService , UserService userService, IMediaPicker mediaPicker)
        {
            this.databaseUserService = databaseUserService;
            this.userService = userService;
            this.mediaPicker = mediaPicker;

            user = userService.user;
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
    }
}
