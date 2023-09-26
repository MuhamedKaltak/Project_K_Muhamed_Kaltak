using Project_K.View;

namespace Project_K
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(RecoverUsernamePage),typeof(RecoverUsernamePage));
        }
    }
}