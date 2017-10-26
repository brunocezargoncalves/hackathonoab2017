using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lexfy.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T Get(Guid entityId);
        List<T> Find(T entity);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SoftDelete(T entity);
        List<SqlParameter> Parameter(T entity);
    }
}