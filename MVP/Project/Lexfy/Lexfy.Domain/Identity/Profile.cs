using System;

namespace Lexfy.Domain.Identity
{
    public class Profile : Person
    {
        public Guid ProfileId { get; set; }

        public Profile()
        {
            ProfileId = Guid.NewGuid();
        }
    }
}