using Project_K.ViewModel;

namespace Project_K.View;

public partial class ChangeEmailPage : ContentPage
{
	public ChangeEmailPage(ChangeEmailViewModel changeEmailViewModel)
	{
		InitializeComponent();

		BindingContext = changeEmailViewModel;
	}
}