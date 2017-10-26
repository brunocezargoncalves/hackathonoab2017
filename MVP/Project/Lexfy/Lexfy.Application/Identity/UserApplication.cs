using System;
using System.Collections.Generic;
using Lexfy.Application.Identity.Interfaces;
using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;

namespace Lexfy.Application.Identity
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileApplication _profileApplication;

        public UserApplication(IUserRepository userRepository,
                               IProfileApplication profileApplication)
        {
            _userRepository = userRepository;
            _profileApplication = profileApplication;
        }        

        public User Get(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public List<User> Find(User entity)
        {
            throw new NotImplementedException();
        }

        public Guid Save(User entity)
        {
            throw new NotImplementedException();
        }

        public void SoftDelete(User entity)
        {
            throw new NotImplementedException();
        }

        public User ForgotPassword(string userName)
        {
            try
            {
                var user = _userRepository.ForgotPassword(userName);

                if (user != null)
                    user.Profile = _profileApplication.Get(user.ProfileId);

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
