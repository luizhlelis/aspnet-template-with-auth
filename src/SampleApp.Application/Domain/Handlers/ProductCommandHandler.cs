using SampleApp.Application.Commands;
using SampleApp.Application.Ports;

namespace SampleApp.Application.Domain.Handlers;

public class ProductCommandHandler : IProductDrivingPort
{
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public ProductCommandHandler(IDatabaseDrivenPort databaseDrivenPort)
    {
        _databaseDrivenPort = databaseDrivenPort;
    }

    public Task Handle(AddProductCommand command)
    {
        throw new NotImplementedException();
    }
}
