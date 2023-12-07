namespace TuberTreats.Models;

public class TuberDriver 
 {
    public int Id { get; set; }
    public string Name { get; set; }
    //TuberDeliveries represents a list of TuberOrder objects associated with a specific TuberDriver. 
    //This property signifies the deliveries handled or assigned to that particular driver.
    public List<TuberOrder> TuberDeliveries { get; set;}
 }