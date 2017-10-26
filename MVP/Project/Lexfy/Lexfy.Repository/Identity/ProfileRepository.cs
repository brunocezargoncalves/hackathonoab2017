using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;

namespace Lexfy.Repository.Identity
{
    public class ProfileRepository : Context<Profile>, IProfileRepository
    {
        public override Profile Adaptar(DataRow dataRow)
        {
            throw new NotImplementedException();
        }

        public override List<Profile> Adaptar(DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        public void Add(Profile entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Profile entity)
        {
            throw new NotImplementedException();
        }

        public List<Profile> Find(Profile entity)
        {
            throw new NotImplementedException();
        }

        public Profile Get(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(Profile entity)
        {
            throw new NotImplementedException();
        }

        public void SoftDelete(Profile entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Profile entity)
        {
            throw new NotImplementedException();
        }
    }
}