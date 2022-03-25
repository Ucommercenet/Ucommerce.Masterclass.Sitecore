using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Masterclass.Models;

namespace Ucommerce.Masterclass.Sitecore.Controllers
{
    public class MasterClassBasketController : Controller
    {
        private static ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            var basket = TransactionLibrary.GetBasket(false);
            var billingInformation = TransactionLibrary.GetBillingInformation();
            var shippingInformation = TransactionLibrary.GetShippingInformation();

            var model = new PurchaseOrderViewModel
            {
                BillingAddress = MapAddresses(billingInformation),
                DiscountTotal = basket.DiscountTotal.ToString(),
                OrderLines = MapOrderLines(basket),
                OrderTotal = basket.OrderTotal.ToString(),
                PaymentTotal = basket.PaymentTotal.ToString(),
                ShippingAddress = MapAddresses(shippingInformation),
                ShippingTotal = basket.ShippingTotal.ToString(),
                SubTotal = basket.SubTotal.ToString(),
                TaxTotal = basket.TaxTotal.ToString()
            };

            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(int quantity, int orderlineId)
        {
            TransactionLibrary.UpdateLineItemByOrderLineId(orderlineId, quantity);
            TransactionLibrary.ExecuteBasketPipeline();

            return Index();
        }

        private List<OrderlineViewModel> MapOrderLines(Ucommerce.EntitiesV2.PurchaseOrder basket)
        {
            return basket.OrderLines.ToList().Select(orderLine => new OrderlineViewModel
            {
                Quantity = orderLine.Quantity,
                ProductName = orderLine.ProductName,
                Discount = orderLine.Discount,
                Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString(),
                OrderLineId = orderLine.OrderLineId,
                Sku = orderLine.Sku,
                UnitPrice = new Money(orderLine.Price, basket.BillingCurrency.ISOCode).ToString(),
                VariantSku = orderLine.VariantSku,
                Tax = orderLine.VAT.ToString()
            }).ToList();
        }

        private AddressViewModel MapAddresses(OrderAddress addressInformation)
        {
            return new AddressViewModel
            {
                FirstName = addressInformation.FirstName ?? string.Empty,
                LastName = addressInformation.LastName ?? string.Empty,
                City = addressInformation.City ?? string.Empty,
                CountryName = addressInformation.Country == null ? string.Empty : addressInformation.Country.Name,
                EmailAddress = addressInformation.EmailAddress ?? string.Empty,
                State = addressInformation.State ?? string.Empty,
                PhoneNumber = addressInformation.PhoneNumber ?? string.Empty,
                PostalCode = addressInformation.PostalCode ?? string.Empty
            };
        }
    }
}