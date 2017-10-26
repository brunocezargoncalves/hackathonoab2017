using Lexfy.Domain.Identity;
using Lexfy.Repository.Interfaces;

namespace Lexfy.Repository.Identity.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User ForgotPassword(string userName);
    }
}