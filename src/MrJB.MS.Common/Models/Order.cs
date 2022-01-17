namespace MrJB.MS.Common.Models;

public class Order
{
    public int? OrderId { get; set; }

    public CustomerAddress BillingAddress { get; set; }

    public CustomerAddress ShippingAddress { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }
}
