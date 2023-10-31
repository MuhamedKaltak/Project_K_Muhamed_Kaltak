using Project_K.View;

namespace Project_K
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(RecoverUsernamePage),typeof(RecoverUsernamePage));
            Routing.RegisterRoute(nameof(ResetPasswordEmailPage),typeof(ResetPasswordEmailPage));
            Routing.RegisterRoute(nameof(ResetPasswordTokenPage),typeof(ResetPasswordTokenPage));
            Routing.RegisterRoute(nameof(ResetPasswordPage),typeof(ResetPasswordPage));
            Routing.RegisterRoute(nameof(UserProfilePage),typeof(UserProfilePage));
            Routing.RegisterRoute(nameof(ChangePasswordSecurityCheckPage),typeof(ChangePasswordSecurityCheckPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage),typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(ChangeEmailCurrentEmailTokenPage),typeof(ChangeEmailCurrentEmailTokenPage));
            Routing.RegisterRoute(nameof(ChangeEmailNewEmailPage),typeof(ChangeEmailNewEmailPage));
            Routing.RegisterRoute(nameof(ChangeEmailNewEmailTokenPage),typeof(ChangeEmailNewEmailTokenPage));
        }
    }
}