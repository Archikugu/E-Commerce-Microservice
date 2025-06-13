namespace MultiShop.Cargo.Dtos.Dtos.CargoDetailDtos;

public class CreateCargoDetailDto
{
    public string SenderCustomer { get; set; }
    public string RecieverCustomer { get; set; }
    public int Barcode { get; set; }
    public int CargoCompanyId { get; set; }
}
