using MediatR;
using MinimalApis.Model;

namespace MinimalApis.Commands.Setup2Fa;

public record Setup2FaCommand(Guid id):IRequest<ResultModel<Setup2FaCommandDto>>;