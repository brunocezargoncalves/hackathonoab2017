using System;
using System.Collections.Generic;

namespace Lexfy.Domain
{
    public class Node
    {
        public Guid NodeId { get; set; }
        public Guid BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public List<Tag> Tags { get; set; }
    }
}