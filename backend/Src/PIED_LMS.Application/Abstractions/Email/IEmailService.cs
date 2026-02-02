using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIED_LMS.Contract.Abstractions.Email;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
