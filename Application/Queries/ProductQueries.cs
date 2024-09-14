using Application.DTOs;
using MediatR;

namespace Application.Queries;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; set; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}
