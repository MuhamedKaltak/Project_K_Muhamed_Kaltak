using Project_K.ViewModel;

namespace Project_K.View;

public partial class ResetPasswordEmailPage : ContentPage
{
	public ResetPasswordEmailPage(ResetPasswordViewModel resetPasswordViewModel)
	{
		InitializeComponent();
		BindingContext = resetPasswordViewModel;
	}
}