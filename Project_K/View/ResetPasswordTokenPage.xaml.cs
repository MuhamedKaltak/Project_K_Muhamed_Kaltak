using Project_K.ViewModel;

namespace Project_K.View;

public partial class ResetPasswordTokenPage : ContentPage
{
	public ResetPasswordTokenPage(ResetPasswordViewModel resetPasswordViewModel)
	{
		InitializeComponent();
		BindingContext = resetPasswordViewModel;
	}
}