using Domain.DTO;
using Microsoft.AspNetCore.Identity;



namespace Application.Contracts;

public interface IAuthenticationService
{
    Task<TokenDto> CreateTokenAsync(bool expireTime);
    Task<TokenDto> RefreshTokenAsync(TokenDto token);
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration);
    Task<bool> ValidateUserAsync(UserForAuthenticationDto user);
}
