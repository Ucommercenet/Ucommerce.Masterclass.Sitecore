using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Masterclass.Models;
using Ucommerce.Search.Facets;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;

namespace Ucommerce.Masterclass.Sitecore.Controllers
{
    public class MasterClassCategoryController : Controller
    {
        private static ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        private static ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        private static IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(string sku)
        {
            return Index();
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            var currentCategory = CatalogContext.CurrentCategory;
            if (currentCategory == null) throw new NullReferenceException();

            var products = CatalogLibrary.GetProducts(currentCategory.Guid);

            var categoryModel = new CategoryViewModel
            {
                Name = currentCategory.Name,
                ImageMediaUrl = currentCategory.ImageMediaUrl,
                TotalProductsCount = products.TotalCount,
                Products = MapProducts(products.Results)
            };

            return View(categoryModel);
        }

        private IList<FacetsViewModel> MapFacets(IList<Facet> facets)
        {
            var facetsToReturn = new List<FacetsViewModel>();

            return facetsToReturn;
        }

        private static IList<ProductViewModel> MapProducts(IList<Product> products)
        {
            var productPrices = CatalogLibrary.CalculatePrices(products.Select(p => p.Guid).ToList());

            return products.Select(product => new ProductViewModel
            {
                ShortDescription = product.ShortDescription,
                LongDescription = product.LongDescription,
                Sku = product.Sku,
                PrimaryImageUrl = product.PrimaryImageUrl,
                IsVariant = product.ProductType == ProductType.Variant,
                Sellable = product.ProductType == ProductType.Variant || product.ProductType == ProductType.Product,
                Prices = productPrices.Items.Where(item =>
                        item.ProductGuid == product.Guid &&
                        item.PriceGroupGuid == CatalogContext.CurrentPriceGroup.Guid)
                    .ToList(),
                Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[]
                {
                    CatalogContext.CurrentCategory
                }, product)
            }).ToList();
        }
    }

    public static class FacetedQueryStringExtensions
    {
        public static IList<Facet> ToFacets(this NameValueCollection target)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var queryString in HttpContext.Current.Request.QueryString.AllKeys)
            {
                parameters[queryString] = HttpContext.Current.Request.QueryString[queryString];
            }

            var facetsForQuerying = new List<Facet>();

            foreach (var parameter in parameters.Where(x =>
                !(new[] { "product", "variant", "category", "categories", "catalog" }.Contains(x.Key))))
            {
                var facet = new Facet { FacetValues = new List<FacetValue>(), Name = parameter.Key };
                foreach (var value in parameter.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    facet.FacetValues.Add(new FacetValue() { Value = value });
                }

                facetsForQuerying.Add(facet);
            }

            return facetsForQuerying;
        }
    }
}