﻿using Microsoft.EntityFrameworkCore.Storage;
using System;
using Wiz.Biblioteca.Domain.Interfaces.UoW;
using Wiz.Biblioteca.Infra.Context;

namespace Wiz.Biblioteca.Infra.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EntityContext _entityContext;
        private IDbContextTransaction _transaction;

        public UnitOfWork(EntityContext entityContext)
        {
            _entityContext = entityContext;
        }

        public int Commit()
        {
            return _entityContext.SaveChanges();
        }

        public void BeginTransaction()
        {
            _transaction = _entityContext.Database.BeginTransaction();
        }

        public void BeginCommit()
        {
            _transaction.Commit();
        }

        public void BeginRollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _entityContext.Dispose();

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
