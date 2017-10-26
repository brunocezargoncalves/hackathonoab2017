using Lexfy.Application.Identity.Interfaces;
using Lexfy.Domain.Identity;
using System;
using System.Collections.Generic;

namespace Lexfy.Application.Identity
{
    public class ProfileApplication : IProfileApplication
    {
        public List<Profile> Find(Profile entity)
        {
            throw new NotImplementedException();
        }

        public Profile Get(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public Guid Save(Profile entity)
        {
            throw new NotImplementedException();
        }

        public void SoftDelete(Profile entity)
        {
            throw new NotImplementedException();
        }
    }
}
