using Project_K.ViewModel;

namespace Project_K.View;

public partial class ChangePasswordPage : ContentPage
{
	public ChangePasswordPage(ChangePasswordViewModel changePasswordViewModel)
	{
		InitializeComponent();

		BindingContext = changePasswordViewModel;
    }
}