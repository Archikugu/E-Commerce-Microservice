namespace MultiShop.Cargo.Entities.Concrete;

public class CargoOperation
{
    public int CargoOperationId { get; set; }
    public string Barcode { get; set; }
    public string Description { get; set; }
    public DateTime OperationDate { get; set; }
} 