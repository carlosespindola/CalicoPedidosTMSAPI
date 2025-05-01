using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BrainSystem.Pedidos.Model;
//using AuthAdminDBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BrainSystem.Auth.API.Identity
{
    //public interface IBrainSystemUserStore<TUser> : IUserStore<CLIENTESACCESOAPI>
    //{
    //    Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken);
    //}

    public class UserStore : IUserStore<CLIENTESACCESOAPI>, IUserPasswordStore<CLIENTESACCESOAPI>
    {
        private readonly BrainSystem.Pedidos.DAL.BrainSystemDBContext db;

        public UserStore(BrainSystem.Pedidos.DAL.BrainSystemDBContext db)
        {
            this.db = db;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
        }

        public Task<string> GetUserIdAsync(CLIENTESACCESOAPI user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.CODIGOCLIENTE.ToString());
        }

        public Task<string> GetUserNameAsync(CLIENTESACCESOAPI user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.USUARIOAPI);
        }

        public Task SetUserNameAsync(CLIENTESACCESOAPI user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(SetUserNameAsync));
        }

        public Task<string> GetNormalizedUserNameAsync(CLIENTESACCESOAPI user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(GetNormalizedUserNameAsync));
        }

        public Task SetNormalizedUserNameAsync(CLIENTESACCESOAPI user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult((object)null);
        }

        public async Task<IdentityResult> CreateAsync(CLIENTESACCESOAPI user, CancellationToken cancellationToken)
        {
            db.Add(user);

            await db.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(CLIENTESACCESOAPI user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(UpdateAsync));
        }

        public async Task<IdentityResult> DeleteAsync(CLIENTESACCESOAPI user, CancellationToken cancellationToken)
        {
            db.Remove(user);

            int i = await db.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(i == 1 ? IdentityResult.Success : IdentityResult.Failed());
        }

        public async Task<CLIENTESACCESOAPI> FindByIdAsync(string cliente, CancellationToken cancellationToken)
        {
            return await db.CLIENTESACCESOAPIs.FindAsync(cliente);
        }

        //este metodo es parte de la interfase IUserStore<TUser> x eso se deja aca, como no implementada
        public async Task<CLIENTESACCESOAPI> FindByNameAsync(string normalizedCliente, CancellationToken cancellationToken)
        {
            return await db.CLIENTESACCESOAPIs
                            .FirstOrDefaultAsync<CLIENTESACCESOAPI>(p => p.CODIGOCLIENTE.Equals(normalizedCliente, StringComparison.OrdinalIgnoreCase), cancellationToken);


            //return await db.CLIENTESACCESOAPIs
            //               .AsAsyncEnumerable()
            //               .SingleOrDefault(p => p.CODIGOCLIENTE.Equals(normalizedCliente, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        ////este metodo es el que realmente se utiliza para buscar el cliente+usuario
        ////OJO!! no forma parte de la interfase IUserStore<TUser>
        //public async Task<CLIENTESACCESOAPI> FindByNameAsync(string normalizedCliente, string normalizedUsuario, CancellationToken cancellationToken)
        //{
        //    return await db.CLIENTESACCESOAPIs
        //                   .AsAsyncEnumerable().Where(p => p.CODIGOCLIENTE.Equals(normalizedCliente, StringComparison.OrdinalIgnoreCase) 
        //                                              && (p.USUARIOAPI != null && p.USUARIOAPI.Equals(normalizedUsuario, StringComparison.OrdinalIgnoreCase)
        //                                                  || (p.USUARIOAPI == null && string.IsNullOrEmpty(p.USUARIOAPI))))
        //                   .SingleOrDefault();
        //}

        public Task SetPasswordHashAsync(CLIENTESACCESOAPI user, string passwordHash, CancellationToken cancellationToken)
        {
            //user.PasswordHash = passwordHash;

            return Task.FromResult((object)null);
        }

        public Task<string> GetPasswordHashAsync(CLIENTESACCESOAPI user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PASSWORDAPI); //.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CLIENTESACCESOAPI user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PASSWORDAPI)); //PasswordHash));
        }
    }
}
