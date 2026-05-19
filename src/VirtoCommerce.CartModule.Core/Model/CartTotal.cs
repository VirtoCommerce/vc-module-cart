namespace VirtoCommerce.CartModule.Core.Model;

public class CartTotal
{
    public string CurrencyCode { get; set; }

    public decimal Total { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TaxTotal { get; set; }

    public decimal DiscountTotal { get; set; }
}
