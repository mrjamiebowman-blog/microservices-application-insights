namespace MrJB.MS.Common.Models;
    
public class CustomerAddress
{
    public int? CustomerAddressId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string StreetAddress1 { get; set; }

    public string StreetAddress2 { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string PostalCode { get; set; }

    public string Country { get; set; }
}
