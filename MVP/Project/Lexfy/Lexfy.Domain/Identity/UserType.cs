using System;

namespace Lexfy.Domain.Identity
{
    public class UserType
    {
        public Guid UserTypeId { get; set; }
        public string Name { get; set; }

        public UserType()
        {
            UserTypeId = Guid.NewGuid();
        }
    }
}
