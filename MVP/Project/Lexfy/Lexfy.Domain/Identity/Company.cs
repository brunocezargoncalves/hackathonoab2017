using System;

namespace Lexfy.Domain.Identity
{
    public class Company : Person
    {
        public Guid CompanyId { get; set; }

        public Company()
        {
            CompanyId = Guid.NewGuid();
        }
    }
}