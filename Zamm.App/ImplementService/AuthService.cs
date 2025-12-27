using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Domain.Validation;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Zamm.Application.Handle.HandleEmail;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.RequestModels.UserRequests;
using Zamm.Application.Payloads.ResultModels.User;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Extensions;

namespace Zamm.Application.ImplementService
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmEmailRepository;
        private readonly IBaseRepository<Permissions> _basePermissionsRepository;
        private readonly IBaseRepository<Role> _baseRoleRepository;
        private readonly IBaseRepository<RefreshToken> _baseRefreshTokenRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthService(IBaseRepository<User> baseUserRepository, IConfiguration configuration, IUserRepository userRepository, IEmailService emailService, 
            IBaseRepository<ConfirmEmail> baseConfirmEmailRepository, IBaseRepository<Permissions> basePermissionsRepository, IBaseRepository<Role> baseRoleRepository, 
            IBaseRepository<RefreshToken> baseRefreshTokenRepository, IHttpContextAccessor contextAccessor)
        {
            _baseUserRepository = baseUserRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
            _baseConfirmEmailRepository = baseConfirmEmailRepository;
            _basePermissionsRepository = basePermissionsRepository;
            _baseRoleRepository = baseRoleRepository;
            _baseRefreshTokenRepository = baseRefreshTokenRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<string> ConfirmRegisterAccount(string confirmCode)
        {
            try
            {
                var code = await _baseConfirmEmailRepository.GetAsync(x => x.ConfirmCode.Equals(confirmCode));
                if(code == null)
                {
                    return "Invalid cofirmation code.";
                }
                var user = await _baseUserRepository.GetAsync(x => x.Id == code.UserId);
                if(code.ExpiryTime < DateTime.Now.AddMinutes(14))
                {
                    return "The confirmation code has expired.";
                }
                code.IsConfirmed = true;
                await _baseUserRepository.UpdateAsync(user);
                await _baseConfirmEmailRepository.UpdateAsync(code);
                return "Successfully registered.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #region Private Methods
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInHours"], out int tokenValidityInHours);
            var expirationUTC = DateTime.Now.AddHours(tokenValidityInHours);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationUTC,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
        #endregion

        public async Task<ResponseObject<LoginResult>> GetJwtTokenAsync(User user)
        {
            var permissions = await _basePermissionsRepository.GetAllAsync(x => x.UserId == user.Id);
            var roles = await _baseRoleRepository.GetAllAsync();

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
        
                new Claim("UserId", user.Id.ToString()),
            };

            foreach (var permission in permissions)
            {
                foreach (var role in roles)
                {
                    if (role.Id == permission.RoleId)
                    {
                        authClaims.Add(new Claim("Permission", role.RoleName));
                    }
                }
            }

            var userRoles = await _userRepository.GetRolesOfUserAsync(user);
            foreach (var item in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, item));
            }

            var jwtToken = GetToken(authClaims);
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int refreshTokenValidity);

            var rf = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryTime = DateTime.UtcNow.AddHours(refreshTokenValidity),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _baseRefreshTokenRepository.CreateAsync(rf);

            return new ResponseObject<LoginResult>
            {
                Status = 200,
                Message = "Create token successfully!",
                Data = new LoginResult
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    RefreshToken = refreshToken,
                }
            };
        }


        public async Task<LoginResult> LoginAsync(LoginInput request)
        {
            var user = await _baseUserRepository.GetAsync(x => x.Email == request.Email);
            if (user == null)
            {
                throw new ResponseErrorObject(
                    "Incorrect email.",
                    StatusCodes.Status400BadRequest
                );
            }

            bool checkPass = BCryptNet.Verify(request.Password, user.Password);
            if (!checkPass)
            {
                throw new ResponseErrorObject(
                    "Incorrect password.",
                    StatusCodes.Status400BadRequest
                );
            }

            var jwtTokenResponse = await GetJwtTokenAsync(user);
            if (jwtTokenResponse.Data == null)
            {
                throw new ResponseErrorObject(
                    "Token generation failed",
                    StatusCodes.Status500InternalServerError
                );
            }
            
            var userRoles = await _userRepository.GetRolesOfUserAsync(user);

            return new LoginResult
            {
                UserResult = new UserResult
                {
                    Id = user.Id,
                    Avatar = user.Avatar,
                    DateOfBirth = user.DateOfBirth,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber ?? "",
                    Email = user.Email,
                    Roles = userRoles.ToList()
                },
                AccessToken = jwtTokenResponse.Data.AccessToken,
                RefreshToken = jwtTokenResponse.Data.RefreshToken
            };
        }

        public async Task LogoutAsync()
        {
            var user = _contextAccessor.HttpContext?.User;
            var userId = user.GetUserId();
    
            await _userRepository.RevokeRefreshTokensAsync(userId);
        }

        public async Task<UserResult> GetUserInfoAsync()
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            var userId = currentUser.GetUserId();

            var user = await _baseUserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ResponseErrorObject(
                    "User not found.",
                    StatusCodes.Status404NotFound
                );
            }

            var userRoles = await _userRepository.GetRolesOfUserAsync(user);

            return new UserResult
            {
                Id = user.Id,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth,
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Roles = userRoles.ToList()
            };
        }


        public async Task<UserResult> RegisterAsync(RegisterInput request)
        {
            if (!ValidateInput.IsValidEmail(request.Email))
            {
                throw new ResponseErrorObject(
                    "Invalid email.",
                    StatusCodes.Status400BadRequest
                );
            }

            if (await _userRepository.GetUserByEmail(request.Email) != null)
            {
                throw new ResponseErrorObject(
                    "This email address is already in use.",
                    StatusCodes.Status400BadRequest
                );
            }

            if (await _userRepository.GetUserByUsername(request.UserName) != null)
            {
                throw new ResponseErrorObject(
                    "This username is already in use.",
                    StatusCodes.Status400BadRequest
                );
            }

            try
            {
                var user = new User
                {
                    Avatar =
                        "https://static.vecteezy.com/system/resources/previews/009/292/244/original/default-avatar-icon-of-social-media-user-vector.jpg",
                    DateOfBirth = DateTime.UtcNow,
                    FullName = request.FullName,
                    Password = BCryptNet.HashPassword(request.Password),
                    UserName = request.UserName,
                    Email = request.Email,
                    HasOnboarded = true,
                    CreatedAt = DateTime.UtcNow,
                };

                user = await _baseUserRepository.CreateAsync(user);

                bool roleAdded = await _userRepository.AddRoleToUserAsync(user, new List<string> { "User" });
            
                if (!roleAdded)
                {
                    throw new InvalidOperationException("Failed to assign user role.");
                }

                /*var confirmEmail = new ConfirmEmail
                {
                    IsActive = true,
                    ConfirmCode = GenerateCodeActive(),
                    ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                    IsConfirmed = false,
                    UserId = user.Id
                };

                await _baseConfirmEmailRepository.CreateAsync(confirmEmail);

                var message = new EmailMessage(
                    new[] { request.Email },
                    "Confirm your account",
                    $"Your confirmation code is: {confirmEmail.ConfirmCode}"
                );

                _emailService.SendEmail(message);*/

                var userRoles = await _userRepository.GetRolesOfUserAsync(user);
                return new UserResult
                {
                    Id = user.Id,
                    Avatar = user.Avatar,
                    DateOfBirth = user.DateOfBirth,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Roles = userRoles.ToList()
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private string GenerateCodeActive()
        {
            string str = "QH_" + DateTime.Now.Ticks.ToString();
            return str;
        }

        public async Task<string> AddRolesToUser(Guid userId, List<string> roles)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            try
            {
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return "Invalidate user.";
                }
                //if (!currentUser.IsInRole("Admin"))
                //{
                //    return "Only for admin";
                //}
                var user = await _baseUserRepository.GetByIdAsync(userId);
                if(user == null)
                {
                    return "Unknown user.";
                }
                await _userRepository.AddRoleToUserAsync(user, roles);
                return "Add roles successfully.";
            }
            catch (Exception ex) 
            {
                return ex.Message;
            }
        }

        public Task<string> DeleteRoles(long userId, List<string> roles)
        {
            throw new NotImplementedException();
        }


        public async Task<ResponseObject<UserResult>> UpdateUserAsync(Guid userId, UpdateUserInput request)
        {
            try
            {
                var httpContext = _contextAccessor.HttpContext
                                  ?? throw new ResponseErrorObject("Unauthorized", StatusCodes.Status401Unauthorized);

                var currentUser = httpContext.User;

                if (!currentUser.Identity!.IsAuthenticated)
                {
                    throw new ResponseErrorObject("Unauthorized", StatusCodes.Status401Unauthorized);
                }

                var currentUserId = currentUser.GetUserId();

                if (currentUserId != userId)
                {
                    throw new ResponseErrorObject(
                        "You are not allowed to update this user",
                        StatusCodes.Status403Forbidden
                    );
                }


                var existingUser = await _baseUserRepository.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    throw new ResponseErrorObject("User not found", StatusCodes.Status404NotFound);
                }

                var userByEmail = await _userRepository.GetUserByEmail(request.Email);
                if (!string.IsNullOrWhiteSpace(request.Email) &&
                    userByEmail != null && userByEmail.Id != existingUser.Id)
                {
                    throw new ResponseErrorObject("Email already exists", StatusCodes.Status400BadRequest);
                }

                if(request.DateOfBirth == null)
                {
                    request.DateOfBirth = existingUser.DateOfBirth;
                }


                existingUser.FullName = request.FullName;
                existingUser.DateOfBirth = request.DateOfBirth;
                existingUser.Email = request.Email;
                existingUser.PhoneNumber = request.PhoneNumber;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existingUser);

                var roles = await _userRepository.GetRolesOfUserAsync(existingUser);

                var responseData = new UserResult
                {
                    Id = existingUser.Id,
                    Avatar = existingUser.Avatar,
                    DateOfBirth = existingUser.DateOfBirth,
                    FullName = existingUser.FullName,
                    UserName = existingUser.UserName,
                    PhoneNumber = existingUser.PhoneNumber,
                    Email = existingUser.Email,
                    Roles = roles.ToList()
                };

                return new ResponseObject<UserResult>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "User updated successfully",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                throw new ResponseErrorObject(ex.Message, StatusCodes.Status400BadRequest);
            }
        }


        public async Task<ResponseObject<UserResult>> ChangePasswordAsync(Guid userId, ChangePasswordInput request)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new ResponseErrorObject("User not found", StatusCodes.Status404NotFound);
                }

                bool checkPass = BCryptNet.Verify(request.CurrentPassword, user.Password);
                if (!checkPass)
                {
                    throw new ResponseErrorObject("Incorrect old password", StatusCodes.Status400BadRequest);
                }

                if (!request.NewPassword.Equals(request.ConfirmPassword))
                {
                    throw new ResponseErrorObject("Passwords do not match", StatusCodes.Status400BadRequest);
                }

                user.Password = BCryptNet.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.Now;
                await _baseUserRepository.UpdateAsync(user);
                var roles = await _userRepository.GetRolesOfUserAsync(user);
                var responseData = new UserResult
                {
                    Id = user.Id,
                    Avatar = user.Avatar,
                    DateOfBirth = user.DateOfBirth,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Roles = roles.ToList()
                };
                
                return new ResponseObject<UserResult>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Password changed successfully",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                throw new ResponseErrorObject(ex.Message, StatusCodes.Status400BadRequest);
            }
        }

    }
}
