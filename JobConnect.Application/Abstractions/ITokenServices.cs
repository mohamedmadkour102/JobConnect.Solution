using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions;

public interface ITokenServices
{
    Task<string> CreateTokenAsync(User user);
}
