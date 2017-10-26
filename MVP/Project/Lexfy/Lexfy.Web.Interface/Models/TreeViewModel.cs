using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Lexfy.Web.Interface.Models
{
    public class TreeViewModel
    {
        [ScaffoldColumn(false)]
        public Guid TreeId { get; set; }

        [Required(ErrorMessage = "O campo Título é obrigatório.")]
        [Display(Name = "Título")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "Descrição")]
        [MaxLength(250)]
        public string Description { get; set; }
        
        [Display(Name = "Árvore pai")]
        public string SelectedTreeChild { get; set; }
        public ICollection<SelectListItem> TreesChild { get; set; }
    }
}