
using TuberTreats.Models;
using TuberTreats.Models.DTOs;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

List<Topping> toppings = new List<Topping>
{
    new Topping { Id = 1, Name = "Spud Sprinkles" },
    new Topping { Id = 2, Name = "Tater Tidbits" },
    new Topping { Id = 3, Name = "Mashed Magic" },
    new Topping { Id = 4, Name = "Butter Boogie" },
    new Topping { Id = 5, Name = "Cheesy Chuckles" }
};

List<TuberOrder> tuberOrders = new List<TuberOrder>
{
    new TuberOrder { Id = 1, OrderPlacedOnDate = DateTime.Now, CustomerId = 1, TuberDriverId = null, DeliveredOnDate = DateTime.Now },
    new TuberOrder { Id = 2, OrderPlacedOnDate = DateTime.Now, CustomerId = 2, TuberDriverId = 2, DeliveredOnDate = DateTime.Now },
    new TuberOrder { Id = 3, OrderPlacedOnDate = DateTime.Now, CustomerId = 3, TuberDriverId = 3, DeliveredOnDate = DateTime.Now },
    new TuberOrder { Id = 4, OrderPlacedOnDate = DateTime.Now, CustomerId = 4, TuberDriverId = 1, DeliveredOnDate = DateTime.Now },
    new TuberOrder { Id = 5, OrderPlacedOnDate = DateTime.Now, CustomerId = 5, TuberDriverId = 2, DeliveredOnDate = DateTime.Now }
 //public List<TuberOrder> TuberToppings { get; set;}
};

List<TuberDriver> drivers = new List<TuberDriver>
{
    new TuberDriver { Id = 1, Name = "Tater Turner" },
    new TuberDriver { Id = 2, Name = "Fryin' Brian" },
    new TuberDriver { Id = 3, Name = "Tuber Timmy" }
    // public List<TuberOrder> TuberDeliveries { get; set;}
};

List<Customer> customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Penny Potaterson", Address = "123 Main Street" },
    new Customer { Id = 2, Name = "Chip McFry", Address = "456 Elm Street" },
    new Customer { Id = 3, Name = "Spudnik Jones", Address = "789 Oak Street" },
    new Customer { Id = 4, Name = "Mashed Maxine", Address = "101 Pine Street" },
    new Customer { Id = 5, Name = "Frytastic Freda", Address = "246 Maple Street" }
//public List<TuberOrder> TuberOrders { get; set;}
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping { Id = 1, TuberOrderId = 1, ToppingId = 1 },
    new TuberTopping { Id = 2, TuberOrderId = 1, ToppingId = 3 },
    new TuberTopping { Id = 3, TuberOrderId = 2, ToppingId = 2 },
    new TuberTopping { Id = 4, TuberOrderId = 2, ToppingId = 4 },
    new TuberTopping { Id = 5, TuberOrderId = 3, ToppingId = 5 },
    new TuberTopping { Id = 6, TuberOrderId = 3, ToppingId = 1 }
};


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here--------------------------

app.MapGet("/tuberorders", () =>
{
    // It selects each TuberOrder object (to) and performs a series of operations within the curly braces for each TuberOrder.
    return tuberOrders.Select(to =>
    {
        // Find the Toppings on the current TuberOrder
        // relevant Toppings might return this 
        // e.g. relevantToppings =  { Id = 1, TuberOrderId = 1, ToppingId = 1 }, { Id = 2, TuberOrderId = 1, ToppingId = 3 }

        var relevantToppings = tuberToppings.Where(tt => tt.TuberOrderId == to.Id);

        // Create a list of ToppingDTO objects for the related TuberToppings
        var orderToppings = relevantToppings.Select(tt =>
        {
            // Find the corresponding Topping for each TuberTopping
            var matchingTopping = toppings.FirstOrDefault(t => t.Id == tt.ToppingId);

            // Create a ToppingDTO if a matching Topping is found
            return matchingTopping != null ? new ToppingDTO
            {
                Id = matchingTopping.Id,
                Name = matchingTopping.Name
            } : null; // Return null if no matching Topping is found
        }).ToList();

        // d is a temporary variable representing each element (TuberDriver) in the drivers list while iterating through it.
        // e.g. to.TuberDriverId = 1. the first time the drivers list (d) has a an ID of one, that becomes matching driver
        var matchingDriver = drivers.FirstOrDefault(d => d.Id == to.TuberDriverId);
        // Create and return a TuberOrderDTO containing the associated Toppings
        return new TuberOrderDTO
        {
            Id = to.Id,
            OrderPlacedOnDate = to.OrderPlacedOnDate,
            CustomerId = to.CustomerId,
            TuberDriverId = to.TuberDriverId,
            DeliveredOnDate = to.DeliveredOnDate,
            Toppings = orderToppings,
            Driver = matchingDriver != null ? new TuberDriverDTO
            {
                Id = matchingDriver.Id,
                Name = matchingDriver.Name
            } : null
        };
    });
});

//---------------------------------------------

app.MapPost("/tuberorders", (TuberOrder newTuberOrder) =>
{
    // Assigning the newTuberOrder an ID by iterating through tuber orders and finding the max existing ID and adding 1
    newTuberOrder.Id = tuberOrders.Max(to => to.Id) + 1;
    newTuberOrder.OrderPlacedOnDate = DateTime.Now;
   
    tuberOrders.Add(newTuberOrder);

    //entered in customer id, toppings
    return Results.Created($"/tuberOrders/{newTuberOrder.Id}", new TuberOrderDTO
    {
        Id = newTuberOrder.Id,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = newTuberOrder.CustomerId,
        Toppings = newTuberOrder.Toppings.Select(to => new ToppingDTO
        {
            Id = to.Id,
            Name = to.Name
        }).ToList(),
        TuberDriverId = newTuberOrder.TuberDriverId,
        DeliveredOnDate = newTuberOrder.DeliveredOnDate,
        
    });
    
});

//---------------------------------------------

app.MapPut("/tuberorders/{id}", (int Id) =>
{

    TuberOrder orderToUpdate = tuberOrders.FirstOrDefault(to => to.Id == Id);
    orderToUpdate.TuberDriverId = 2;

});   

//---------------------------------------------

app.MapPost("/tuberorders/{id}", (int Id) => 
{

});





app.MapGet("/tuberorders/{id}", (int Id) =>
{
    TuberOrder tuberOrder = tuberOrders.FirstOrDefault(to => to.Id == Id);

    return new TuberOrderDTO
    {
        Id = tuberOrder.Id,
        OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
        CustomerId = tuberOrder.CustomerId,
        TuberDriverId = tuberOrder.TuberDriverId,
        DeliveredOnDate = tuberOrder.DeliveredOnDate,
        //   Toppings = toppings.Select(t => new ToppingDTO
        // {
        //     Id = t.Id,
        //     Name = t.Name
        // }).ToList()
    };
});

//---------------------------------------------

app.MapGet("/toppings", () =>
{
    return toppings.Select(t => new ToppingDTO
    {
        Id = t.Id,
        Name = t.Name
    });

});

//---------------------------------------------

app.MapGet("/toppings/{id}", (int id) =>
{
    //find matching topping with id
    Topping topping = toppings.FirstOrDefault(t => t.Id == id);

    return new ToppingDTO
    {
        Id = topping.Id,
        Name = topping.Name
    };
});

//---------------------------------------------

app.MapGet("/tubertoppings/", () =>
{
    return tuberToppings.Select(tt => new TuberToppingDTO
    {
        Id = tt.Id,
        TuberOrderId = tt.TuberOrderId,
        ToppingId = tt.ToppingId
    });
});

app.Run();
//don't touch or move this!
public partial class Program { }