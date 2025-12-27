using Zamm.Application.Payloads.ResultModels.User;

namespace Zamm.Application.Payloads.ResultModels.User
{
    public class LoginResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserResult UserResult { get; set; }
    }
}
