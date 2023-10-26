using Project_K.ViewModel;

namespace Project_K.View;

public partial class UserProfilePage : ContentPage
{
	public UserProfilePage(UserProfileViewModel userProfileViewModel)
	{
		InitializeComponent();

		BindingContext = userProfileViewModel;
	}
}