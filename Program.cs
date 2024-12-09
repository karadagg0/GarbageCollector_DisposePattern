using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Press any key to start...");
        Console.ReadLine();

        Console.WriteLine("Creating objects...");
        double totalValue = SimulateOrders();

        Console.WriteLine($"Total Order Value: {totalValue:F2}");

        TriggerGarbageCollection();

        GeneratePersistentData();

        Console.WriteLine("Application finished.");
        Console.ReadLine();
    }

    static double SimulateOrders()
    {
        const int orderCount = 5000;
        double cumulativeValue = 0;

        for (int i = 0; i < orderCount; i++)
        {
            using (var order = new Order($"Order-{i + 1}", GenerateRandomPrice()))
            {
                cumulativeValue += order.Price;
            }
        }
        return cumulativeValue;
    }

    static void GeneratePersistentData()
    {
        var customer = new Customer("Alice Johnson");
        customer.AddOrder(new Order("Order-123", 250.75));
        customer.AddOrder(new Order("Order-456", 330.40));

        Console.WriteLine($"Customer '{customer.Name}' added.");
    }

    static double GenerateRandomPrice() => new Random().Next(100, 1000) + new Random().NextDouble();

    static void TriggerGarbageCollection()
    {
        Console.WriteLine("\nStarting manual GC...");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("GC completed.\n");
    }
}

class Customer
{
    public string Name { get; }
    private readonly List<Order> _orders = new();

    public Customer(string name) => Name = name;

    public void AddOrder(Order order) => _orders.Add(order);

    ~Customer() => Console.WriteLine($"Customer '{Name}' finalized.");
}

class Order : IDisposable
{
    public string Id { get; }
    public double Price { get; }
    private bool _disposed;

    public Order(string id, double price)
    {
        Id = id;
        Price = price;
        Console.WriteLine($"Order '{Id}' created with price: {Price:F2}");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Console.WriteLine($"Order '{Id}' managed resources released.");
        }

        Console.WriteLine($"Order '{Id}' unmanaged resources released.");
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Order()
    {
        Dispose(false);
        Console.WriteLine($"Order '{Id}' finalized.");
    }
}
