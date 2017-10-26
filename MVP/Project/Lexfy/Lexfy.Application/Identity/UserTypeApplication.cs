using Lexfy.Application.Identity.Interfaces;
using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;

namespace Lexfy.Application.Identity
{
    public class UserTypeApplication : IUserTypeApplication
    {
        private readonly IUserTypeRepository _userTypeRepository;

        public UserTypeApplication(IUserTypeRepository userTypeRepository)
        {
            _userTypeRepository = userTypeRepository;
        }

        public UserType Get(Guid userTypeId)
        {
            try
            {
                return _userTypeRepository.Get(userTypeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<UserType> Find(UserType userType)
        {
            try
            {
                return _userTypeRepository.Find(userType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(UserType userType)
        {
            try
            {
                var userTypeId = Guid.Empty;

                // UserType já existe
                if (userType.UserTypeId != Guid.Empty && Get(userType.UserTypeId) != null)
                {
                    // Atualiza
                    _userTypeRepository.Update(new UserType
                    {
                        UserTypeId = userType.UserTypeId,
                        Name = userType.Name
                    });

                    userTypeId = userType.UserTypeId;
                }
                // UserType não existe
                else
                {
                    userTypeId = Guid.NewGuid();

                    // Adiciona novo UserType
                    _userTypeRepository.Add(new UserType
                    {
                        UserTypeId = userTypeId,
                        Name = userType.Name
                    });
                }

                return userTypeId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(UserType userType)
        {
            throw new NotImplementedException();
        }
    }
}
