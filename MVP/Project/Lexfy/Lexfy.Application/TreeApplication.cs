using Lexfy.Application.Interfaces;
using Lexfy.Domain;
using Lexfy.Repository.Interfaces;
using System;
using System.Collections.Generic;

namespace Lexfy.Application
{
    public class TreeApplication : ITreeApplication
    {
        private readonly ITreeRepository _treeRepository;

        public TreeApplication(ITreeRepository treeRepository)
        {
            _treeRepository = treeRepository;
        }

        public Tree Get(Guid treeId)
        {
            try
            {
                return _treeRepository.Get(treeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Tree> Find(Tree tree)
        {
            try
            {
                return _treeRepository.Find(tree);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(Tree tree)
        {
            try
            {
                var treeId = Guid.Empty;

                // Tree já existe
                if (tree.TreeId != Guid.Empty && Get(tree.TreeId) != null)
                {
                    // Atualiza
                    _treeRepository.Update(new Tree
                    {
                        TreeId = tree.TreeId,
                        Title = tree.Title,
                        Description = tree.Description,
                        TreeChildId = tree.TreeChildId
                    });

                    treeId = tree.TreeId;
                }
                // Tree não existe
                else
                {
                    treeId = Guid.NewGuid();

                    // Adiciona novo Tree
                    _treeRepository.Add(new Tree
                    {
                        TreeId = treeId,
                        Title = tree.Title,
                        Description = tree.Description,
                        TreeChildId = tree.TreeChildId
                    });
                }

                return treeId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(Tree tree)
        {
            throw new NotImplementedException();
        }
    }
}
