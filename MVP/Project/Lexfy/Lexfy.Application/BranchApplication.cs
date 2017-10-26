using System;
using System.Collections.Generic;
using Lexfy.Application.Interfaces;
using Lexfy.Domain;
using Lexfy.Repository.Interfaces;

namespace Lexfy.Application
{
    public class BranchApplication : IBranchApplication
    {
        private readonly IBranchRepository _branchRepository;

        public BranchApplication(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public Branch Get(Guid branchId)
        {
            try
            {
                return _branchRepository.Get(branchId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Branch> Find(Branch branch)
        {
            try
            {
                return _branchRepository.Find(branch);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(Branch branch)
        {
            try
            {
                var branchId = Guid.Empty;

                // Branch já existe
                if (branch.BranchId != Guid.Empty && Get(branch.BranchId) != null)
                {
                    // Atualiza
                    _branchRepository.Update(new Branch
                    {
                        BranchId = branch.BranchId,
                        TreeId = branch.TreeId,
                        Index = branch.Index,
                        Title = branch.Title,
                        Description = branch.Description,
                        BranchChildId = branch.BranchChildId
                    });

                    branchId = branch.BranchId;
                }
                // Branch não existe
                else
                {
                    branchId = Guid.NewGuid();

                    // Adiciona novo Branch
                    _branchRepository.Add(new Branch
                    {
                        BranchId = branchId,
                        TreeId = branch.TreeId,
                        Index = branch.Index,
                        Title = branch.Title,
                        Description = branch.Description,
                        BranchChildId = branch.BranchChildId
                    });
                }

                return branchId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(Branch branch)
        {
            throw new NotImplementedException();
        }
    }
}
