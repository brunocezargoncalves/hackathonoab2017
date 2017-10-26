using System;
using System.Collections.Generic;

namespace Lexfy.Application.Interfaces
{
    public interface IApplication<T> where T : class
    {
        T Get(Guid entityId);
        List<T> Find(T entity);
        Guid Save(T entity);
        void SoftDelete(T entity);
    }
}