

namespace Application.Features.Customers.Dtos.Response;

public class CreatedCustomerResponse
{
    public string Name { get; set; }
    public byte[] PasswordHash { get; set; }
}
