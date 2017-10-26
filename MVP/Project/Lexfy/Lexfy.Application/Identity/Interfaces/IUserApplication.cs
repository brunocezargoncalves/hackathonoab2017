using Lexfy.Application.Interfaces;
using Lexfy.Domain.Identity;

namespace Lexfy.Application.Identity.Interfaces
{
    public interface IUserApplication : IApplication<User>
    {
        User ForgotPassword(string userName);
    }
}
