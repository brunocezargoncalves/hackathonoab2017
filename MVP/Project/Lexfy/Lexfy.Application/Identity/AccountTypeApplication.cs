using Lexfy.Application.Identity.Interfaces;
using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;

namespace Lexfy.Application.Identity
{
    public class AccountTypeApplication : IAccountTypeApplication
    {
        private readonly IAccountTypeRepository _accountTypeRepository;

        public AccountTypeApplication(IAccountTypeRepository accountTypeRepository)
        {
            _accountTypeRepository = accountTypeRepository;
        }

        public AccountType Get(Guid accountTypeId)
        {
            try
            {
                return _accountTypeRepository.Get(accountTypeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<AccountType> Find(AccountType accountType)
        {
            try
            {
                return _accountTypeRepository.Find(accountType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(AccountType accountType)
        {
            try
            {
                var accountTypeId = Guid.Empty;

                // AccountType já existe
                if (accountType.AccountTypeId != Guid.Empty && Get(accountType.AccountTypeId) != null)
                {
                    // Atualiza
                    _accountTypeRepository.Update(new AccountType
                    {
                        AccountTypeId = accountType.AccountTypeId,
                        Name = accountType.Name
                    });

                    accountTypeId = accountType.AccountTypeId;
                }
                // AccountType não existe
                else
                {
                    accountTypeId = Guid.NewGuid();

                    // Adiciona novo AccountType
                    _accountTypeRepository.Add(new AccountType
                    {
                        AccountTypeId = accountTypeId,
                        Name = accountType.Name
                    });
                }

                return accountTypeId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(AccountType accountType)
        {
            throw new NotImplementedException();
        }
    }
}
