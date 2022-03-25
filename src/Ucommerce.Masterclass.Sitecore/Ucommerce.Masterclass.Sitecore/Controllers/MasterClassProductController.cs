using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Masterclass.Models;
using Ucommerce.Search.Models;

namespace Ucommerce.Masterclass.Sitecore.Controllers
{
    public class MasterClassProductController : Controller
    {
        
        private static ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        private static ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        private static ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();
        
        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            var currentProduct = CatalogContext.CurrentProduct;
            if (currentProduct == null) throw new NullReferenceException();

            var productModel = new ProductViewModel
            {
                PrimaryImageUrl = currentProduct.PrimaryImageUrl,
                Sku = currentProduct.Sku,
                Name = currentProduct.Name,
                Prices = CatalogLibrary.CalculatePrices(new List<Guid>() { currentProduct.Guid }).Items,
                Variants = MapVariants(CatalogLibrary.GetVariants(currentProduct))
            };

            return View(productModel);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(string sku, string variantSku, int quantity)
        {
            TransactionLibrary.AddToBasket(quantity, sku, variantSku);

            return Index();
        }
        
        private static IList<ProductViewModel> MapVariants(IEnumerable<Product> variants)
        {
            return variants.Select(variant => new ProductViewModel
            {
                Name = variant.DisplayName ?? variant.Name,
                VariantSku = variant.VariantSku
            }).ToList();
        }
    }
}