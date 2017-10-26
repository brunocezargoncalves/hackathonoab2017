using Lexfy.Application.Identity.Interfaces;
using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;

namespace Lexfy.Application.Identity
{
    public class AccountApplication : IAccountApplication
    {
        private readonly IAccountRepository _accountRepository;

        public AccountApplication(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Account Get(Guid accountId)
        {
            try
            {
                return _accountRepository.Get(accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Account> Find(Account account)
        {
            try
            {
                return _accountRepository.Find(account);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(Account account)
        {
            try
            {
                var accountId = Guid.Empty;

                // Account já existe
                if (account.AccountId != Guid.Empty && Get(account.AccountId) != null)
                {
                    // Atualiza
                    _accountRepository.Update(new Account
                    {
                        AccountId = account.AccountId,
                        CompanyId = account.CompanyId
                    });

                    accountId = account.AccountId;
                }
                // Account não existe
                else
                {
                    accountId = Guid.NewGuid();

                    // Adiciona novo Account
                    _accountRepository.Add(new Account
                    {
                        AccountId = accountId,
                        CompanyId = account.CompanyId
                    });
                }

                return accountId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
