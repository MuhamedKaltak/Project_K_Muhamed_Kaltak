using Project_K.ViewModel;

namespace Project_K.View;

public partial class ChangeEmailNewEmailPage : ContentPage
{
	public ChangeEmailNewEmailPage(UserProfileViewModel userProfileViewModel)
	{
		InitializeComponent();

		BindingContext = userProfileViewModel;
	}
}