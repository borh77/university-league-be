using AutoMapper;
using Solution.Identity.API.Dtos;
using Solution.Identity.API.Public;
using Solution.Identity.Core.Domain.RepositoryInterfaces;

namespace Solution.Identity.Core.UseCases;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IMapper _mapper;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _mapper = mapper;
    }

    public AuthResponseDto Login(LoginRequestDto request)
    {
        if (request is null
            || string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Password))
            throw new UnauthorizedAccessException("Invalid username or password.");

        var user = _userRepository.GetByUsername(request.Username.Trim());

        // Nepostojeci, deaktiviran ili pogresna lozinka - ista poruka da se ne otkriva koji nalog postoji
        if (user is null
            || !user.IsActive
            || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password.");

        var token = _tokenGenerator.Generate(user);

        var response = _mapper.Map<AuthResponseDto>(user);
        response.Token = token.Token;
        response.ExpiresAt = token.ExpiresAt;
        return response;
    }
}
