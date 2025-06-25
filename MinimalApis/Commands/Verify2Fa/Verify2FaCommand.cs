using MediatR;
using MinimalApis.Model;

namespace MinimalApis.Commands.Verify2Fa;

public record Verify2FaCommand(Guid id, string code): IRequest<ResultModel>;
