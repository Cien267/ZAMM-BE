using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zamm.Application.Handle.HandleEmail;

namespace Zamm.Application.InterfaceService
{
    public interface IEmailService
    {
         string SendEmail(EmailMessage emailMessage);
    }
}
