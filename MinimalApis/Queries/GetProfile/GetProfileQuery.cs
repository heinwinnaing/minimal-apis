using MediatR;
using MinimalApis.Model;

namespace MinimalApis.Queries.GetProfile;

public class GetProfileQuery
    : IRequest<ResultModel<GetProfileDto>>
{
    public Guid Id { get; set; }
}
