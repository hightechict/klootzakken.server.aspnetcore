using System.Threading.Tasks;

namespace Klootzakken.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
