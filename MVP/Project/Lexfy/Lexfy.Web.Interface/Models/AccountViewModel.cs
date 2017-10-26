using Lexfy.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Lexfy.Web.Interface.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "O campo E-mail é obrigatório.")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [StringLength(100, ErrorMessage = "A {0} precisa ter pelo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "A Senha não foi confirmada corretamente.")]
        public string ConfirmPassword { get; set; }

        public Guid ResetToken { get; set; }
    }

    public class UserViewModel
    {
        public Guid UserId { get; set; }
        public Guid ProfileId { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [Display(Name = "Nome")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo E-mail é obrigatório.")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [StringLength(100, ErrorMessage = "A {0} precisa ter pelo menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "A Senha não foi confirmada corretamente.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "O campo Tipo é obrigatório.")]
        [Display(Name = "Tipo")]
        public ICollection<SelectListItem> Types { get; set; }
        public string SelectedUserType { get; set; }

        [Display(Name = "Situação")]
        public bool IsActive { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefone")]
        public string Phone { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Celular")]
        public string CellPhone { get; set; }

        [Display(Name = "Departamento")]
        public ICollection<SelectListItem> Departments { get; set; }
        public string SelectedDepartment { get; set; }

        [Display(Name = "Redefinir Senha?")]
        public bool RedefinePassword { get; set; }

        public Guid CreatorUserId { get; set; }

        public UserViewModel()
        {
            UserId = Guid.Empty;
            ProfileId = Guid.Empty;

            Types = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Administrador", Value = "Administrador" },
                new SelectListItem { Text = "Jornalista", Value = "Jornalista" }
            };

            Departments = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Núcleo de Comunicação e Marketing - NCM", Value = "Núcleo de Comunicação e Marketing - NCM" }
            };

            CreatorUserId = Guid.Empty;
        }
    }
}