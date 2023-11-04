using Project_K.View;

namespace Project_K
{
    public partial class AppShellMain : Shell
    {
        public AppShellMain()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(UserProfilePage),typeof(UserProfilePage));
            Routing.RegisterRoute(nameof(ChangePasswordSecurityCheckPage),typeof(ChangePasswordSecurityCheckPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage),typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(ChangeEmailCurrentEmailTokenPage),typeof(ChangeEmailCurrentEmailTokenPage));
            Routing.RegisterRoute(nameof(ChangeEmailNewEmailPage),typeof(ChangeEmailNewEmailPage));
            Routing.RegisterRoute(nameof(ChangeEmailNewEmailTokenPage),typeof(ChangeEmailNewEmailTokenPage));
        }
    }
}