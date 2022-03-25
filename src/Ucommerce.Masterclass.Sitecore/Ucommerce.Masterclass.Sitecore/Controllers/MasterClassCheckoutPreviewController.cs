using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Masterclass.Models;

namespace Ucommerce.Masterclass.Sitecore.Controllers
{
    public class MasterClassCheckoutPreviewController : Controller
    {
        private static ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            var basket = TransactionLibrary.GetBasket();

            var model = new PurchaseOrderViewModel
            {
                BillingAddress = MapAddress(TransactionLibrary.GetBasket().BillingAddress),
                DiscountTotal = basket.DiscountTotal.ToString(),
                OrderLines = basket.OrderLines.Select(x => new OrderlineViewModel
                {
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Sku = x.Sku,
                    Tax = x.VAT.ToString(),
                    Total = x.Total.ToString(),
                    ProductName = x.ProductName,
                    VariantSku = x.VariantSku,
                    OrderLineId = x.OrderLineId,
                    UnitPrice = x.Price.ToString()
                }).ToList(),
                OrderTotal = basket.OrderTotal.ToString(),
                PaymentTotal = basket.PaymentTotal.ToString(),
                ShippingAddress = MapAddress(basket.GetShippingAddress(Constants.DefaultShipmentAddressName)), // MAKE THIS WORK WITH SHIPPING ADDRESS
                ShippingTotal = basket.ShippingTotal.ToString(),
                SubTotal = basket.SubTotal.ToString(),
                TaxTotal = basket.TaxTotal.ToString()
            };

            return View(model);
        }

        private AddressViewModel MapAddress(OrderAddress address)
        {
            var addressModel = new AddressViewModel();
            
            addressModel.FirstName = address.FirstName;
            addressModel.EmailAddress = address.EmailAddress;
            addressModel.LastName = address.LastName;
            addressModel.PhoneNumber = address.PhoneNumber;
            addressModel.MobilePhoneNumber = address.MobilePhoneNumber;
            addressModel.Line1 = address.Line1;
            addressModel.Line2 = address.Line2;
            addressModel.PostalCode = address.PostalCode;
            addressModel.City = address.City;
            addressModel.State = address.State;
            addressModel.Attention = address.Attention;
            addressModel.CompanyName = address.CompanyName;
            addressModel.CountryName = address.Country.Name;

            return addressModel;
        }


        [HttpPost]
        public ActionResult Index(int complete)
        {
            var firstPayment = TransactionLibrary.GetBasket(false).Payments.First();

            return Redirect(TransactionLibrary.GetPaymentPageUrl(firstPayment));
        }
    }
}