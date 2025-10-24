namespace BackendIntegrationTests.TestData;
public static class ProductData
{
    public static object ValidProduct => new
    {
        name = "Samsung A80",
        price = 1233,
        stock = 15
    };
    
    public static object InvalidProduct => new
    {
        name = "",
        price = -1,
        stock = -5
    };
}