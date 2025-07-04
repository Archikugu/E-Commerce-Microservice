﻿namespace MultiShop.Cargo.Dtos.Dtos.CargoCustomerDtos;

public class CreateCargoCustomerDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName.ToUpper()}";
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string District { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
}
