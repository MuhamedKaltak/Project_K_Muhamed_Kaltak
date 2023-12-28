using Project_K.View;

namespace Project_K
{
    public partial class AppShellMain : Shell
    {
        public AppShellMain()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(UserProfilePage),typeof(UserProfilePage));
            Routing.RegisterRoute(nameof(ChangePasswordPage),typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(ChangeEmailPage),typeof(ChangeEmailPage));
        }
    }
}