using MediatR;
using Application.Commands;
using Domain.Interfaces;

namespace Application.Handlers;
public class LoginCommandHandler : IRequestHandler<LoginCommand, string?>
{
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate user credentials
        var user = await _userRepository.GetUserAsync(request.Username, request.Password);
        if (user == null)
        {
            // Invalid user credentials
            return null;
        }

        // Step 2: Generate JWT token using repository method
        var token = _userRepository.GenerateJwtToken(user);
        return token;
    }
}

