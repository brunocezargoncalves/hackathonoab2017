using System;
using System.Collections.Generic;
using Lexfy.Application.Interfaces;
using Lexfy.Domain;
using Lexfy.Repository.Interfaces;

namespace Lexfy.Application
{
    public class NodeApplication : INodeApplication
    {
        private readonly INodeRepository _nodeRepository;

        public NodeApplication(INodeRepository nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public Node Get(Guid nodeId)
        {
            try
            {
                return _nodeRepository.Get(nodeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Node> Find(Node node)
        {
            try
            {
                return _nodeRepository.Find(node);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(Node node)
        {
            try
            {
                var nodeId = Guid.Empty;

                // Node já existe
                if (node.NodeId != Guid.Empty && Get(node.NodeId) != null)
                {
                    // Atualiza
                    _nodeRepository.Update(new Node
                    {
                        NodeId = node.NodeId,
                        BranchId = node.BranchId
                    });

                    nodeId = node.NodeId;
                }
                // Node não existe
                else
                {
                    nodeId = Guid.NewGuid();

                    // Adiciona novo Node
                    _nodeRepository.Add(new Node
                    {
                        NodeId = nodeId,
                        BranchId = node.BranchId
                    });
                }

                return nodeId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SoftDelete(Node node)
        {
            throw new NotImplementedException();
        }
    }
}
