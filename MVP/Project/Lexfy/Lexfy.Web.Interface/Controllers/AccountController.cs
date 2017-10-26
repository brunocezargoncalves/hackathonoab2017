using Lexfy.Application.Communication.Interfaces;
using Lexfy.Application.Identity.Interfaces;
using Lexfy.Domain.Identity;
using Lexfy.Web.Interface.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace Lexfy.Web.Interface.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserApplication _userApplication;
        private readonly INotificationApplication _notificationApplication;

        public AccountController(IUserApplication userApplication,
                                 INotificationApplication notificationApplication)
        {
            _userApplication = userApplication;
            _notificationApplication = notificationApplication;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new UserViewModel());
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel, string returnUrl)
        {
            var message = string.Empty;

            if (ModelState.IsValid)
            {
                var user = _userApplication.Find(new User()
                {
                    UserId = Guid.Empty,
                    ProfileId = Guid.Empty,
                    UserName = loginViewModel.Email,
                    IsActive = true
                });

                if (user != null && user.Count > 0 && user[0].PasswordHash == Helpers.HashMD5(loginViewModel.Password))
                {
                    // Id, nome, e-mail, departamento, tipo o perfil, data de crição
                    FormsAuthentication.SetAuthCookie($"{ user[0].UserId }|{ user[0].Profile.Name}|{ user[0].UserName }|{ user[0].Type }", false);
                    return Redirect(returnUrl ?? Url.Action("Index", "PressAdvisoryService"));
                }

                message = "Verifique seu e-mail e senha e tente novamente.";
            }
            else
            {
                if (string.IsNullOrEmpty(loginViewModel.Email) && string.IsNullOrEmpty(loginViewModel.Password))
                    message = "O endereço de e-mail inserido não pertence a uma conta. Verifique e tente novamente.";

                else if (string.IsNullOrEmpty(loginViewModel.Password))
                    message = "Sua senha está incorreta. Confira-a.";
            }

            ModelState.AddModelError("LoginErrorMessage", message);
            return View(loginViewModel);
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            try
            {
                // Valida o formulário recuperação de senha
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("ForgotPasswordErrorMessage", string.IsNullOrEmpty(forgotPasswordViewModel.Email) ? "Nenhum endereço de e-mail foi inserido." : "O endereço de e-mail inserido é inválido.");
                    return View(forgotPasswordViewModel);
                }

                // Busca usuário do e-mail informado no formulário de recuperação de senha
                var user = _userApplication.ForgotPassword(forgotPasswordViewModel.Email);

                if (user == null)
                    throw new Exception($"O endereço de e-mail { forgotPasswordViewModel.Email } não pertence a uma conta ativa. Verifique e tente novamente.");

                // Variáveis para serem enviadas no e-mail de recuperação
                user.ResetToken = Guid.NewGuid();
                user.ResetTokenExpiration = DateTime.Now.AddHours(1);

                try
                {
                    // Enviar e-mail com link para reset de senha
                    Helpers.SendMail(new Dictionary<string, string>
                    {
                        { "to", user.Profile.Email },
                        { "subject", "Esqueci minha senha" },
                        { "body", MessageMailForgotPassword(user.UserId, user.Profile.Name, user.Profile.Email, (Guid)user.ResetToken, (DateTime)user.ResetTokenExpiration) }
                    });

                    // Registra solicitação de recuperação de senha
                    user.Profile = null; // Não atualizar Profile                    
                    _userApplication.Save(user);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Problema ao registrar a solicitação de redefinição de senha. Tente novamente, caso o problema persista, informe o suporte: { ex.Message }");
                }

                ViewData["ForgotPasswordViewModel.Email"] = forgotPasswordViewModel.Email;
                return View("ForgotPasswordConfirmation");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ForgotPasswordErrorMessage", ex.Message);
                return View(forgotPasswordViewModel);
            }
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string resetToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(resetToken))
                {
                    var user = _userApplication.Get(Guid.Parse(userId));

                    if (user != null)
                    {
                        if (user.ResetToken.ToString() != resetToken || DateTime.Parse(user.ResetTokenExpiration.ToString()) < DateTime.Now)
                            throw new Exception("Essa solicitação de redefinição de senha não é válida!");

                        return View(new ResetPasswordViewModel { UserId = user.UserId, Email = user.UserName, ResetToken = Guid.Parse(resetToken) });
                    }
                    else
                        throw new Exception("Usuário não encontrado ou inativado!");
                }

                throw new Exception("Essa solicitação de redefinição de senha não é válida!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ResetPasswordErrorMessage", ex.Message);
                return View();
            }
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            try
            {
                // Valida o formulário recuperação de senha
                if (!ModelState.IsValid)
                    throw new Exception("Campos não enviados corretamente. Verifique e tente novamente.");

                var user = _userApplication.Get(resetPasswordViewModel.UserId);

                if (user != null)
                {
                    user.Profile = null;
                    user.PasswordHash = Helpers.HashMD5(resetPasswordViewModel.Password);
                    _userApplication.Save(user);
                }
                else
                    throw new Exception("Desculpe, não foi possível concluir a redefinição de senha. Abra o link recebido e tente novamente.");

                return View("ResetPasswordConfirmation");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ResetPasswordErrorMessage", ex.Message);
                return View();
            }
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/Details/5
        public ActionResult Details(string userId)
        {
            return View(FillFields(userId));
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserViewModel userViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(userViewModel);

                if (_userApplication.Find(new User()
                {
                    UserId = Guid.Empty,
                    ProfileId = Guid.Empty,
                    UserName = userViewModel.Email
                }).Count > 0)
                {
                    throw new Exception($"Já existe um usuário cadastrado com o endereço de e-mail { userViewModel.Email }.");
                }

                Save("Register", userViewModel);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserErrorMessage", ex.Message);
                return View(userViewModel);
            }
        }

        // GET: Account/Edit/5
        public ActionResult Edit(string userId)
        {
            return View(FillFields(userId));
        }

        // POST: Account/Edit/5
        [HttpPost]
        public ActionResult Edit(UserViewModel userViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Se senha inválida e usuário decidir não mais redefinir a senha
                    if (userViewModel.Password.Length < 6 || userViewModel.Password != userViewModel.ConfirmPassword)
                    {
                        var user = _userApplication.Get(userViewModel.UserId);
                        return View(new UserViewModel()
                        {
                            UserId = userViewModel.UserId,
                            ProfileId = userViewModel.ProfileId,
                            Name = userViewModel.Name,
                            Email = userViewModel.Email,
                            Password = user.PasswordHash,
                            ConfirmPassword = user.PasswordHash,
                            SelectedUserType = userViewModel.SelectedUserType,
                            IsActive = userViewModel.IsActive,
                            Phone = userViewModel.Phone,
                            CellPhone = userViewModel.CellPhone,
                            SelectedDepartment = userViewModel.SelectedDepartment,
                            CreatorUserId = userViewModel.CreatorUserId
                        });
                    }

                    return View(userViewModel);
                }

                Save("Edit", userViewModel);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserErrorMessage", ex.Message);
                return View(userViewModel);
            }
        }

        // GET: Account/Delete/5
        public ActionResult Delete(string userId)
        {
            return View(FillFields(userId));
        }

        // POST: Account/Delete/5
        [HttpPost]
        public ActionResult Delete(UserViewModel userViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(userViewModel);

                Save("Delete", userViewModel);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UserErrorMessage", ex.Message);
                return View(userViewModel);
            }
        }

        // GET: Account/UpdateHistory/5
        public ActionResult UpdateHistory(string pressAdvisoryServiceId)
        {
            return View();
        }

        #region Helpers

        public JsonResult Search()
        {
            var users = _userApplication.Find(null).Select(user => new string[]
            {
                user.UserId.ToString(),
                user.Profile.Name,
                user.UserName
            }).ToList();

            return Json(new
            {
                aaData = users
            }, JsonRequestBehavior.AllowGet);
        }

        private UserViewModel FillFields(string userId)
        {
            var user = _userApplication.Get(Guid.Parse(userId));

            if (user != null)
            {
                return new UserViewModel()
                {
                    UserId = user.UserId,
                    ProfileId = user.ProfileId,
                    Name = user.Profile.Name,
                    Email = user.Profile.Email,
                    Password = user.PasswordHash,
                    ConfirmPassword = user.PasswordHash,
                    //SelectedUserType = user.UserTypeId,
                    IsActive = user.IsActive,
                    Phone = user.Profile.Phone,
                    CellPhone = user.Profile.CellPhone
                };
            }

            return new UserViewModel();
        }

        private void Save(string caller, UserViewModel userViewModel)
        {
            try
            {
                var authenticatedUser = Helpers.GetAuthenticatedUser();

                var user = new User()
                {
                    UserId = userViewModel.UserId,
                    UserName = userViewModel.Email,
                    PasswordHash = Helpers.HashMD5(userViewModel.Password),

                    Profile = new Profile()
                    {
                        ProfileId = userViewModel.ProfileId,
                        Name = userViewModel.Name,
                        Phone = userViewModel.Phone,
                        CellPhone = userViewModel.CellPhone,
                        Email = userViewModel.Email
                    },

                    //Type = userViewModel.SelectedType,
                    IsActive = userViewModel.IsActive
                };

                switch (caller)
                {
                    case "Create":
                    case "Register":
                        _userApplication.Save(user);
                        break;
                    case "Edit":
                        _userApplication.Save(user);
                        break;
                    case "Delete":
                        _userApplication.SoftDelete(user);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string MessageMailForgotPassword(Guid userId, string name, string email, Guid resetToken, DateTime resetTokenExpiration)
        {
            NameValueCollection environmentSettings = (NameValueCollection)ConfigurationManager.GetSection("environment");
            var environment = environmentSettings[ConfigurationManager.AppSettings["environment"]];

            var messageHtml = new StringBuilder();
            messageHtml.Append($"<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html style=\"margin: 0; padding: 0;\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title>Lexfy by Fecomércio PR - Notificação</title><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /><meta name=\"viewport\" content=\"width=device-width\" /></head><body><table cellpadding=\"0\" cellspacing=\"0\" style=\"border-radius:4px; border:1px #3c8dbc solid\" border=\"0\" align=\"center\"><tbody><tr><td colspan=\"3\" height=\"6\"></td></tr><tr style=\"line-height: 0px\"><td width=\"100%\" style=\"font-size: 0px\" align=\"center\" height=\"1\"><img width=\"200px\" style=\"max-height: 80px; width: 200px\" alt=\"\" src=\"{ environment }/content/images/logo-email.png\" /></td></tr><tr><td><table cellpadding=\"0\" cellspacing=\"0\" style=\"line-height:25px\" border=\"0\" align=\"center\"><tbody><tr><td colspan=\"3\" height=\"30\"></td></tr><tr><td width=\"36\"></td><td width=\"454\" align=\"left\" style=\"color: #444444; border-collapse: collapse; font-size: 10.2pt; font-family: 'Open Sans','Lucida Grande','Segoe UI',Arial,Verdana,'Lucida Sans Unicode',Tahoma,'Sans Serif'; max-width:454px;\" valign=\"top\">");
            messageHtml.Append($"Olá, { name.Split(' ')[0] }!<br />Tudo bem?<br /><br />");

            messageHtml.Append($"Recebemos uma solitação de redefinição de senha para seu usuário Lexfy <span style=\"text-decoration:none\"><a href=\"mailto: bruno@somainterativa.com.br\" target=\"_blank\" style=\"text-decoration: none;\">{ email }</a></span>.<br /><br />");
            messageHtml.Append($"Para dar continuidade <a href=\"{ environment }/Account/ResetPassword?userId={ userId }&resetToken={ resetToken }\" style=\"text-decoration: none;\">acesse esse link</a>. A solicitação é válida até { resetTokenExpiration }.<br /><br />");

            messageHtml.Append($"Esperamos ter ajudado!<br />- Equipe Lexfy<br /></td><td width=\"36\"> </td></tr><tr> <td colspan=\"3\" height=\"36\"> </td> </tr></tbody></table></td></tr></tbody></table></body></html>");

            return messageHtml.ToString();
        }

        #endregion Helpers
    }
}