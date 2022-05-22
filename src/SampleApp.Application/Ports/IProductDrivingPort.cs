using SampleApp.Application.Commands;

namespace SampleApp.Application.Ports;

public interface IProductDrivingPort
{
    public Task Handle(AddProductCommand command);
}
