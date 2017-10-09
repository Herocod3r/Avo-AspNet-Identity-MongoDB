using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace Avo.AspNet.Identity.MongoDB
{
    /// <summary>
    /// UserStore base class which implements all you need for a basic user store in identity
    /// This implementation uses a generic string, allowing the developer to decide, at point of use, which type
    /// to use for their entity ID keys
    /// </summary>
    public class UserStore<TUser> :
        IUserStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserLoginStore<TUser>,
        IUserPasswordStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserTwoFactorStore<TUser, string>, IUserRoleStore<TUser>
        where TUser : IdentityUser
    {
        private readonly IMongoCollection<TUser> _userCollection;

        public UserStore(IMongoCollection<TUser> userCollection)
        {
            _userCollection = userCollection;
        }

        public virtual Task CreateAsync(TUser user)
        {
            return _userCollection.InsertOneAsync(user);
        }

        public virtual Task UpdateAsync(TUser user)
        {
            return _userCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public virtual Task DeleteAsync(TUser user)
        {
            return _userCollection.DeleteOneAsync(u => u.Id == user.Id);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            return Task.Run(()=> _userCollection.Find(u => u.Id == userId).FirstOrDefault());
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            return _userCollection.Find(u => u.UserName == userName).FirstOrDefaultAsync();
        }

        public IQueryable<TUser> Users => _userCollection.AsQueryable();

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            return Task.Run(()=> user.PasswordHash = passwordHash);
        }

        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.Run(()=> user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.Run(() => !string.IsNullOrEmpty(user.PasswordHash));
        }

        public virtual Task SetEmailAsync(TUser user, string email)
        {
            return Task.Run(()=> user.Email = email);
        }

        public virtual Task<string> GetEmailAsync(TUser user)
        {
            return Task.Run(()=>user.Email);
        }

        public virtual Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.Run(() => user.EmailConfirmed);
        }

        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.Run(()=> user.EmailConfirmed = confirmed);
        }

        public virtual Task<TUser> FindByEmailAsync(string email)
        {
            return _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            return Task.Run(()=> user.SecurityStamp = stamp);
        }

        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.Run(() => user.SecurityStamp);
        }

        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            return Task.Run(()=> user.TwoFactorEnabled = enabled);
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.Run(() => user.TwoFactorEnabled);
        }

        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            return Task.Run(()=> user.PhoneNumber = phoneNumber);
        }

        public virtual Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.Run(() => user.PhoneNumber);
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.Run(() => user.PhoneNumberConfirmed);
        }

        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.Run(()=> user.PhoneNumberConfirmed = confirmed);
        }

        public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return Task.Run(() => user.LockoutEndDateUtc.HasValue
                ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                : new DateTimeOffset());
        }

        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            return Task.Run(()=> user.LockoutEndDateUtc = lockoutEnd.UtcDateTime);
        }

        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return Task.Run(() =>
            {
                user.AccessFailedCount++;
                return user.AccessFailedCount;
            });
        }

        public virtual Task ResetAccessFailedCountAsync(TUser user)
        {
            return Task.Run(()=> user.AccessFailedCount = 0);
        }

        public virtual Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.Run(() => user.AccessFailedCount);
        }

        public virtual Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.Run(() => user.LockoutEnabled);
        }

        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            return Task.Run(()=> user.LockoutEnabled = enabled);
        }

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return Task.Run(()=> user.Logins.Add(login));
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            return Task.Run(() =>
            {
                var item = user.Logins.FirstOrDefault(l =>
                    l.LoginProvider == login.LoginProvider
                    && l.ProviderKey == login.ProviderKey);
                user.Logins.Remove(item);
            });
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return Task.Run(() => (IList<UserLoginInfo>)user.Logins);
        }

        public virtual Task<TUser> FindAsync(UserLoginInfo login)
        {
            return _userCollection
                .Find(u => u.Logins.Any(l => 
                        l.LoginProvider == login.LoginProvider 
                        && l.ProviderKey == login.ProviderKey))
                .FirstOrDefaultAsync();
        }

        public virtual void Dispose()
        {
            
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            return Task.Run(() => user.Roles.Add(roleName));
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            return Task.Run(() => user.Roles.Remove(roleName));
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.Run(() => (IList<string>)user.Roles);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Task.Run(() => user.Roles.Contains(roleName));
        }
    }
}