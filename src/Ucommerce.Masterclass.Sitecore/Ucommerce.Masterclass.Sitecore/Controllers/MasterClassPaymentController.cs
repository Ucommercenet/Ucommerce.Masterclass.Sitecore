using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Masterclass.Models;

namespace Ucommerce.Masterclass.Sitecore.Controllers
{
    public class MasterClassPaymentController : Controller
    {
        private static ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            var availablePaymentMethods = TransactionLibrary.GetPaymentMethods(TransactionLibrary.GetShippingInformation().Country);
            var selectedPaymentMethod = TransactionLibrary.GetBasket().Payments.FirstOrDefault()?.PaymentMethod?.PaymentMethodId ?? 0;

            var paymentViewModel = new PaymentViewModel
            {
                AvailablePaymentMethods = availablePaymentMethods.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.PaymentMethodId.ToString(),
                    Selected = selectedPaymentMethod == x.PaymentMethodId,
                }).ToList()
            };

            return View(paymentViewModel);
        }


        [HttpPost]
        public ActionResult Index(int selectedPaymentMethodId)
        {
            TransactionLibrary.CreatePayment(selectedPaymentMethodId);

            TransactionLibrary.ExecuteBasketPipeline();

            return Redirect("/preview");
        }
    }
}