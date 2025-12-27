using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zamm.Domain.Entities;

namespace Zamm.Domain.InterfaceRepositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByPhoneNumber (string username);
        Task<User> GetUserById(Guid Id);
        Task<bool> AddRoleToUserAsync(User user, List<string> Roles);
        Task<IEnumerable<string>> GetRolesOfUserAsync(User user);

        Task<User> UpdateAsync(User user);
        Task RevokeRefreshTokensAsync(Guid userId);

    }
}
