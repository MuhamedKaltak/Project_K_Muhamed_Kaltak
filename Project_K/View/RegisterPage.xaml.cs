using Project_K.ViewModel;

namespace Project_K.View;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel registerViewModel)
	{
		InitializeComponent();

		BindingContext = registerViewModel;
	}
}