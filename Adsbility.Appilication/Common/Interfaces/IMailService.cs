using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Adsbility.Appilication.Common.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
