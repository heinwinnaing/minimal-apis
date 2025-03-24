using MediatR;
using MinimalApis.Model;

namespace MinimalApis.Commands.UpdateProfile;

public class UpdateProfileCommand
    : IRequest<ResultModel>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}
