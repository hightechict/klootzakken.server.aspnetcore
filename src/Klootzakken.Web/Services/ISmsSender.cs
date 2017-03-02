using System.Threading.Tasks;

namespace Klootzakken.Web.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
