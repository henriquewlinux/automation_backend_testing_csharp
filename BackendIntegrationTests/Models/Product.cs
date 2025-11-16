namespace BackendIntegrationTests.Models.TestData;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public int Stock { get; set; }

    public Product WithUniqueName()
    {
        return new Product
        {
            Name = $"{Name} - {DateTime.Now:yyyyMMddHHmmss}",
            Price = Price,
            Stock = Stock
        };
    }
}
