using System;
using System.Collections.Generic;

namespace Lexfy.Domain
{
    public class Branch
    {
        public Guid BranchId { get; set; }
        public Guid TreeId { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid BranchChildId { get; set; }
        public virtual List<Node> Nodes { get; set; }
    }
}