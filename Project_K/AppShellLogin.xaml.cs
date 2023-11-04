using Project_K.View;

namespace Project_K
{
    public partial class AppShellLogin : Shell
    {
        public AppShellLogin()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(RecoverUsernamePage),typeof(RecoverUsernamePage));
            Routing.RegisterRoute(nameof(ResetPasswordEmailPage),typeof(ResetPasswordEmailPage));
            Routing.RegisterRoute(nameof(ResetPasswordTokenPage),typeof(ResetPasswordTokenPage));
            Routing.RegisterRoute(nameof(ResetPasswordPage),typeof(ResetPasswordPage));
        }
    }
}