using Project_K.ViewModel;

namespace Project_K.View;

public partial class ChangePasswordSecurityCheckPage : ContentPage
{
	public ChangePasswordSecurityCheckPage(UserProfileViewModel userProfileViewModel)
	{
		InitializeComponent();
		BindingContext = userProfileViewModel;
	}
}