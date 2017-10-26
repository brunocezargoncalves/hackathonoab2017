using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lexfy.Web.Interface.Models
{
    public class BranchViewModel
    {
        [ScaffoldColumn(false)]
        public Guid BranchId { get; set; }

        public Guid TreeId { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid BranchChildId { get; set; }
    }
}