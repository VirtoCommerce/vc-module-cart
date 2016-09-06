using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Data.Common;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Domain.Tax.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartTaxEvaluatorImpl : IShoppingCartTaxEvaluator
    {
        private readonly IStoreService _storeService;
        public ShoppingCartTaxEvaluatorImpl(IStoreService storeService)
        {
            _storeService = storeService;
        }
        #region IShoppingCartTaxEvaluator Members
        public void EvaluateTaxes(ShoppingCart cart)
        {
            var taxRates = EvaluateCartTaxRates(cart, null);
            ApplyTaxRates(cart, taxRates);
        }

        public void EvaluateTaxes(ShoppingCart cart, IEnumerable<ShippingRate> shippingRates)
        {
            var taxRates = EvaluateCartTaxRates(cart, shippingRates);
            foreach (var shippingRate in shippingRates)
            {
                ApplyTaxRates(shippingRate, taxRates);
            }
        }
        #endregion

        protected virtual IEnumerable<TaxRate> EvaluateCartTaxRates(ShoppingCart cart, IEnumerable<ShippingRate> shippingRates)
        {
            var retVal = new List<TaxRate>();
            var store = _storeService.GetById(cart.StoreId);
            if (store != null)
            {
                var activeTaxProvider = store.TaxProviders.FirstOrDefault(x => x.IsActive);
                if (activeTaxProvider != null)
                {
                    var taxEvaluationContext = CartToTaxEvaluationContext(store, cart, shippingRates);
                    retVal = activeTaxProvider.CalculateRates(taxEvaluationContext).ToList();
                }
            }
            return retVal;
        }

        protected virtual TaxEvaluationContext CartToTaxEvaluationContext(Store store, ShoppingCart cart, IEnumerable<ShippingRate> shippingRates)
        {
            var retVal = new TaxEvaluationContext
            {
                Id = cart.Id,
                Code = cart.Name,
                Currency = cart.Currency,
                Type = "Cart",
                Store = store,
                Lines = new List<TaxLine>()
            };

            if (cart.Items != null)
            {
                foreach (var lineItem in cart.Items)
                {
                    var listTaxLine = new TaxLine
                    {
                        Id = lineItem.Id + "&list",
                        Code = lineItem.Sku,
                        Name = lineItem.Name,
                        TaxType = lineItem.TaxType,
                        Amount = lineItem.ListPrice
                    };
                    retVal.Lines.Add(listTaxLine);

                    if (lineItem.ListPrice != lineItem.SalePrice)
                    {
                        var saleTaxLine = new TaxLine
                        {
                            Id = lineItem.Id + "&sale",
                            Code = lineItem.Sku,
                            Name = lineItem.Name,
                            TaxType = lineItem.TaxType,
                            Amount = lineItem.SalePrice
                        };
                        retVal.Lines.Add(saleTaxLine);
                    }
                }
            }

            if (cart.Shipments != null)
            {
                foreach (var shipment in cart.Shipments)
                {                 
                    var priceTaxLine = new TaxLine
                    {
                        Id = shipment.Id + "&price",
                        Code = shipment.ShipmentMethodCode,
                        Name = shipment.ShipmentMethodCode,
                        TaxType = shipment.TaxType,
                        Amount = shipment.ShippingPrice
                    };
                    retVal.Lines.Add(priceTaxLine);

                    if (shipment.DeliveryAddress != null)
                    {
                        retVal.Address = shipment.DeliveryAddress;
                        retVal.Address.AddressType = shipment.DeliveryAddress.AddressType;
                    }

                    retVal.Customer = new Contact
                    {
                        Id = cart.CustomerId,
                        Name = cart.CustomerName
                    };
                }
            }

            if(!shippingRates.IsNullOrEmpty())
            {
                foreach (var shippingRate in shippingRates)
                {
                    var taxLine = new TaxLine
                    {
                        Id = string.Join("&", shippingRate.ShippingMethod.Code, shippingRate.OptionName),
                        Code = string.Join("&", shippingRate.ShippingMethod.Code, shippingRate.OptionName),
                        Name = string.Join("&", shippingRate.ShippingMethod.Name, shippingRate.OptionDescription),
                        TaxType = shippingRate.ShippingMethod.TaxType,
                        Amount = shippingRate.Rate
                    };
                    retVal.Lines.Add(taxLine);
                }
            }
            return retVal;
        }

        protected virtual void ApplyTaxRates(ShoppingCart shoppingCart, IEnumerable<TaxRate> taxRates)
        {        
            if (shoppingCart.Items != null)
            {
                foreach (var lineItem in shoppingCart.Items)
                {
                    ApplyTaxRates(lineItem, taxRates);
                }
            }

            if (shoppingCart.Shipments != null)
            {
                foreach (var shipment in shoppingCart.Shipments)
                {
                    ApplyTaxRates(shipment, taxRates);
                }
            }
        }

        protected virtual void ApplyTaxRates(ShippingRate shippingRate, IEnumerable<TaxRate> taxRates)
        {
            var shippingMethodTaxRates = taxRates.Where(x => x.Line.Id.SplitIntoTuple('&').Item1 == shippingRate.ShippingMethod.Code && x.Line.Id.SplitIntoTuple('&').Item2 == shippingRate.OptionName);

            shippingRate.RateWithTax = shippingRate.Rate;

            var shippingMethodTaxRate = shippingMethodTaxRates.FirstOrDefault();
            if (shippingMethodTaxRate != null)
            {
                shippingRate.RateWithTax += shippingMethodTaxRate.Rate;
            }
        }

        protected virtual void ApplyTaxRates(Shipment shipment, IEnumerable<TaxRate> taxRates)
        {
            shipment.ShippingPriceWithTax = shipment.ShippingPrice;

            //Because TaxLine.Id may contains composite string id & extra info
            var shipmentTaxRates = taxRates.Where(x => x.Line.Id.SplitIntoTuple('&').Item1 == shipment.Id).ToList();

            if (shipmentTaxRates.Any())
            {
                var priceTaxRate = shipmentTaxRates.First(x => x.Line.Id.SplitIntoTuple('&').Item2.EqualsInvariant("price"));
                shipment.ShippingPriceWithTax = shipment.ShippingPrice + priceTaxRate.Rate;
            }
        }

        protected virtual void ApplyTaxRates(LineItem lineItem, IEnumerable<TaxRate> taxRates)
        {
            lineItem.ListPriceWithTax = lineItem.ListPrice;
            lineItem.SalePriceWithTax = lineItem.SalePrice;

            //Because TaxLine.Id may contains composite string id & extra info
            var lineItemTaxRates = taxRates.Where(x => x.Line.Id.SplitIntoTuple('&').Item1 == (lineItem.Id ?? "")).ToList();

            if (lineItemTaxRates.Any())
            {
                var listPriceRate = lineItemTaxRates.First(x => x.Line.Id.SplitIntoTuple('&').Item2.EqualsInvariant("list"));
                var salePriceRate = lineItemTaxRates.FirstOrDefault(x => x.Line.Id.SplitIntoTuple('&').Item2.EqualsInvariant("sale"));
                if (salePriceRate == null)
                {
                    salePriceRate = listPriceRate;
                }
                lineItem.ListPriceWithTax = lineItem.ListPrice + listPriceRate.Rate;
                lineItem.SalePriceWithTax = lineItem.SalePrice + salePriceRate.Rate;
            }
        }
    }
}
