using System;
using System.Collections.Generic;

namespace Lexfy.Domain
{
    public class Tree
    {
        public Guid TreeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid TreeChildId { get; set; }
        public virtual List<Branch> Branches { get; set; }

        public Tree()
        {
            TreeId = Guid.NewGuid();
            TreeChildId = Guid.Empty;
            Branches = new List<Branch>();
        }
    }
}