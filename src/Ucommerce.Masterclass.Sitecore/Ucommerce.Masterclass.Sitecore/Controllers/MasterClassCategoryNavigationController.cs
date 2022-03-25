using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Masterclass.Models;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;

namespace Ucommerce.Masterclass.Sitecore.Controllers
{
    public class MasterClassCategoryNavigationController : Controller
    {
        private static ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        private static ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        private static IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        public ActionResult CategoryNavigation()
        {
            var model = new CategoryNavigationViewModel
            {
                Categories = MapCategories(CatalogLibrary.GetRootCategories().Results),
                CurrentCategoryGuid = CatalogContext.CurrentCategory?.Guid ?? Guid.NewGuid()
            };

            return View("/views/MasterClassCategoryNavigation/index.cshtml", model);
        }

        private static IList<CategoryViewModel> MapCategories(IEnumerable<Category> categories)
        {
            return categories.Select(category => new CategoryViewModel
            {
                Name = category.Name,
                Guid = category.Guid,
                Description = category.Description,
                Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new []
                {
                    category
                }),
                Categories = MapCategories(CatalogLibrary.GetCategories(category.Categories).ToList())
            }).ToList();
        }
    }
}