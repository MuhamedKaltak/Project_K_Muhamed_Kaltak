using Project_K.ViewModel;

namespace Project_K.View;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel loginViewModel)
	{
		InitializeComponent();

		BindingContext = loginViewModel;
	}
}