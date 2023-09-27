using CommunityToolkit.Mvvm.Input;
using Project_K.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class RecoverPasswordViewModel : BaseViewModel
    {
        EmailService emailService;
        SecurityService securityService;


        public RecoverPasswordViewModel(SecurityService securityService, EmailService emailService)
        {
            this.securityService = securityService;
            this.emailService = emailService;
        }


    }
}
