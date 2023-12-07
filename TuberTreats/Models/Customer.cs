namespace TuberTreats.Models;

public class CustomerDTO 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    //The property name CustomerOrders is a list of TuberOrder objects
    public List<TuberOrder> TuberOrders { get; set;}
}
