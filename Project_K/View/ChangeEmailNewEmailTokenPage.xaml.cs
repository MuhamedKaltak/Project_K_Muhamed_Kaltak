using Project_K.ViewModel;

namespace Project_K.View;

public partial class ChangeEmailNewEmailTokenPage : ContentPage
{
	public ChangeEmailNewEmailTokenPage(UserProfileViewModel userProfileViewModel)
	{
		InitializeComponent();

		BindingContext = userProfileViewModel;
	}
}