using System.ComponentModel.DataAnnotations;

namespace SampleApp.Application.Domain.Entities;

public class Product : IdentifiableEntity
{
    public Product(string name, double price, int amountAvailable)
    {
        Name = name;
        Price = price;
        AmountAvailable = amountAvailable;
    }

    [Required] [MaxLength(50)] public string Name { get; private set; }
    
    [Required] public double Price { get; private set; }

    [Required] public double AmountAvailable { get; private set; }
    
    public void RemoveItemFromStock()
    {
        AmountAvailable--;
    }
}
