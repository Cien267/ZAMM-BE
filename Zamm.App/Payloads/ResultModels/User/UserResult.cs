using System.Linq.Expressions;
using Zamm.Application.Payloads.ResultModels.Common;

namespace Zamm.Application.Payloads.ResultModels.User
{
    public class UserResult : DataResponseBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Avatar { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public List<string> Roles { get; set; }

        public static Expression<Func<Domain.Entities.User, UserResult>> FromUser =>
            x => new UserResult
            {
                Id = x.Id,
                UserName = x.UserName,
                Avatar = x.Avatar,
                DateOfBirth = x.DateOfBirth,
                Email = x.Email,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
            };
    }
}
