var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


// Mock customer data
var customers = new[]
{
    new Customer(Guid.CreateVersion7().ToString(), "John Doe", "john.doe@example.com", "New York"),
    new Customer(Guid.CreateVersion7().ToString(), "Jane Smith", "jane.smith@example.com", "Los Angeles"),
    new Customer(Guid.CreateVersion7().ToString(), "Bob Johnson", "bob.johnson@example.com", "Chicago"),
    new Customer(Guid.CreateVersion7().ToString(), "Alice Williams", "alice.williams@example.com", "Houston"),
    new Customer(Guid.CreateVersion7().ToString(), "Charlie Brown", "charlie.brown@example.com", "Phoenix")
};

// Get all customers
app.MapGet("/customers", () => customers)
   .WithName("GetAllCustomers");

// Get customer by ID
app.MapGet("/customers/{id}", (string id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    return customer is not null ? Results.Ok(customer) : Results.NotFound();
})
.WithName("GetCustomerById");

app.Run();

internal record Customer(string Id, string Name, string Email, string City);
