using Zamm.Domain.Entities;
using Zamm.Application.Payloads.RequestModels.UserRequests;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Payloads.ResultModels.User;

namespace Zamm.Application.InterfaceService
{
    public interface IAuthService
    {
        Task<UserResult> RegisterAsync(RegisterInput request);
        Task<string> ConfirmRegisterAccount(string confirmCode);
        Task<ResponseObject<LoginResult>> GetJwtTokenAsync(User user);
        Task<LoginResult> LoginAsync(LoginInput request);

        Task<ResponseObject<UserResult>> UpdateUserAsync(Guid userId, UpdateUserInput request);

        Task<UserResult> GetUserInfoAsync();
        Task<ResponseObject<UserResult>> ChangePasswordAsync(Guid userId, ChangePasswordInput request);
        Task<string> AddRolesToUser(Guid userId, List<string> roles);
        Task LogoutAsync();

        Task<List<UserResult>> GetAllUserAsync();
    }
}
