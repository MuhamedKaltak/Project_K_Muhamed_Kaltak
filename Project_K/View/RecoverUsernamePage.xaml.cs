using Project_K.ViewModel;

namespace Project_K.View;

public partial class RecoverUsernamePage : ContentPage
{
	public RecoverUsernamePage(RecoverUsernameViewModel recoveryViewModel)
	{
		InitializeComponent();

		BindingContext = recoveryViewModel;
	}
}