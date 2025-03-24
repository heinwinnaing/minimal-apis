using MediatR;
using MinimalApis.Model;

namespace MinimalApis.Commands.RegisterProfile;

public class RegisterProfileCommand
    : IRequest<ResultModel<RegisterProfileCommandDto>>
{
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
}
