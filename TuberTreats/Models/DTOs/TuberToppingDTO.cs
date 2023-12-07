namespace TuberTreats.Models.DTOs;

public class TuberToppingDTO
{
    public int Id { get; set; }
    public int TuberOrderId { get; set; }
    public int ToppingId { get; set; }
}

//each class is one topping
//class is a template for one