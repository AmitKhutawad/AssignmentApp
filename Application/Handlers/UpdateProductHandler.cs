using Application.Commands;
using Domain.Interfaces;
using MediatR;

namespace Application.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _repository;

    public UpdateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetProductAsync(request.Id);
        if (product == null)
            return false;

        product.Name = request.Name;
        product.Price = request.Price;

        await _repository.UpdateProductAsync(product);
        return true;
    }
}

