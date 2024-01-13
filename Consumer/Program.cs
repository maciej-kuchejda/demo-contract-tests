// See https://aka.ms/new-console-template for more information
using Consumer.Repository;
using Shared.Models;

Console.WriteLine("Hello, World!");
IList<OrderDTO> orders = new List<OrderDTO>();

await using (var repository = new OrderRepository("http://localhost:9001"))
{
    Console.WriteLine("Acquiring Orders");
    orders = await repository.GetAllAsync();
}


foreach (var item in orders)
{
    Console.WriteLine($"Order id: {item.Id}");
    Console.WriteLine($"Name: {item.Name}, CreationDate: {item.CreationDate}, Status: {item.Status}");
    Console.WriteLine("-----------");
}

Console.ReadLine();