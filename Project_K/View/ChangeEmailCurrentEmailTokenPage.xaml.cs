using Project_K.ViewModel;

namespace Project_K.View;

public partial class ChangeEmailCurrentEmailTokenPage : ContentPage
{
	public ChangeEmailCurrentEmailTokenPage(UserProfileViewModel userProfileViewModel)
	{
		InitializeComponent();

		BindingContext = userProfileViewModel;
	}
}