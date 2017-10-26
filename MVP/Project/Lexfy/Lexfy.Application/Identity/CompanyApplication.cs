using Lexfy.Application.Identity.Interfaces;
using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;

namespace Lexfy.Application.Identity
{
    public class CompanyApplication : ICompanyApplication
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyApplication(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public Company Get(Guid companyId)
        {
            try
            {
                return _companyRepository.Get(companyId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Company> Find(Company company)
        {
            try
            {
                return _companyRepository.Find(company);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(Company company)
        {
            try
            {
                var companyId = Guid.Empty;

                // Company já existe
                if (company.CompanyId != Guid.Empty && Get(company.CompanyId) != null)
                {
                    // Atualiza
                    _companyRepository.Update(new Company
                    {
                        CompanyId = company.CompanyId
                    });

                    companyId = company.CompanyId;
                }
                // Company não existe
                else
                {
                    companyId = Guid.NewGuid();

                    // Adiciona novo Company
                    _companyRepository.Add(new Company
                    {
                        CompanyId = companyId
                    });
                }

                return companyId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(Company company)
        {
            throw new NotImplementedException();
        }
    }
}
