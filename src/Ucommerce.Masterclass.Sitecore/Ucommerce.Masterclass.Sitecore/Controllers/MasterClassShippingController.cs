using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Masterclass.Models;

namespace Ucommerce.Masterclass.Sitecore.Controllers
{
    public class MasterClassShippingController : Controller
    {
        private static ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            var availableShippingMethods = TransactionLibrary.GetShippingMethods(TransactionLibrary.GetShippingInformation().Country);
            var selectedShippingMethodId = TransactionLibrary.GetBasket().Shipments.FirstOrDefault()?.ShippingMethod?.ShippingMethodId ?? 0;
            
            var shippingViewModel = new ShippingViewModel
            {
                AvailableShippingMethods = availableShippingMethods.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ShippingMethodId.ToString(),
                    Selected = selectedShippingMethodId == x.ShippingMethodId
                }).ToList()
            };

            return View(shippingViewModel);
        }

        [HttpPost]
        public ActionResult Index(int selectedShippingMethodId)
        {
            TransactionLibrary.CreateShipment(selectedShippingMethodId, Constants.DefaultShipmentAddressName);

            TransactionLibrary.ExecuteBasketPipeline();

            return Redirect("/payment");
        }
    }
}