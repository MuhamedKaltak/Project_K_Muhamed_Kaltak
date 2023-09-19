using CommunityToolkit.Mvvm.Input;
using Project_K.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        [RelayCommand]
        async Task NavigateToRegisterPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(RegisterPage)}");
        }
    }
}
