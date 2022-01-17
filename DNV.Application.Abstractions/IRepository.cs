﻿using DNVGL.Domain.Seedwork;

namespace DNV.Application.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>: IReadonlyRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork StartWorkUnit(bool autoCommit = true);

        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
