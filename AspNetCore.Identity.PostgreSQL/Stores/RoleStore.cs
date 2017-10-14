using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using AspNetCore.Identity.PostgreSQL.Context;
using AspNetCore.Identity.PostgreSQL.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace AspNetCore.Identity.PostgreSQL.Stores
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity role store iterfaces.
    /// </summary>
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private RoleTable<TRole> _roleTable;
        public PostgreSQLDatabase Database { get;  }
        private bool _disposed;

        public IQueryable<TRole> Roles
        {
            get { return _roleTable.GetRoles().AsQueryable(); }
        }


        /// <summary>
        /// Default constructor that initializes a new PostgreSQLDatabase instance using the Default Connection string.
        /// </summary>


        /// <summary>
        /// Constructor that takes a IConfiguration as argument.
        /// </summary>
        /// <param name="config"></param>
        public RoleStore(IConfiguration config)
        {
            this.Database = new PostgreSQLDatabase(config);
            this._roleTable = new RoleTable<TRole>(Database);
        }

        public void Dispose()
        {
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            await Task.Run(() =>
            {
                _roleTable.Insert(role);
            }, cancellationToken);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult("");
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult("");
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult("");
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
