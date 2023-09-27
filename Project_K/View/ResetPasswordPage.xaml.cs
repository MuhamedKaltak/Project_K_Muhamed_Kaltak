using Project_K.ViewModel;

namespace Project_K.View;

public partial class ResetPasswordPage : ContentPage
{
	public ResetPasswordPage(ResetPasswordViewModel resetPasswordViewModel)
	{
		InitializeComponent();
		BindingContext = resetPasswordViewModel;
	}
}