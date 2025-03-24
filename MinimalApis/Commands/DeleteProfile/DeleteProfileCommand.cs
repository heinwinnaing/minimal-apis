using MediatR;
using MinimalApis.Model;

namespace MinimalApis.Commands.DeleteProfile;

public class DeleteProfileCommand
    : IRequest<ResultModel>
{
    public Guid Id { get; set; }
}
