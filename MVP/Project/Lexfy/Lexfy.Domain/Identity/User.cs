using System;

namespace Lexfy.Domain.Identity
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; }

        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public Guid UserTypeId { get; set; }
        public virtual UserType Type { get; set; }

        public bool IsConfirmed { get; set; }
        public bool IsActive { get; set; }

        public Guid? ResetToken { get; set; }
        public DateTime? ResetTokenExpiration { get; set; }

        public User()
        {
            UserId = Guid.NewGuid();
            ProfileId = Guid.NewGuid();
        }
    }
}