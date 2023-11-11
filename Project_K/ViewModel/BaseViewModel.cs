using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string title;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        ImageSource imageToShowTabBar;

        [RelayCommand]
        protected async Task NavigateBackToPreviousPage()
        {
            await Shell.Current.GoToAsync("..");
        }

        public void TabBarRefreshImage_WorkAround() //Workaround for .NET MAUI BUG, images not showing when setting ImageSource to another picture and reappearing to the same tab
        {
            ImageSource temp = ImageToShowTabBar;

            ImageToShowTabBar = null;

            ImageToShowTabBar = temp;
        }
    }
}
