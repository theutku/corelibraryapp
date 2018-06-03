using System.Collections.Generic;
using System.Linq;
using Library.Models.Catalog;
using Library.Models.CheckoutModels;
using LibraryData.Interfaces;
using LibraryData.Models;
using Microsoft.AspNetCore.Mvc;


namespace Library.Controllers
{
    public class CatalogController : Controller
    {

        private readonly ILibraryAsset _assets;
        private readonly ICheckOut _checkouts;

        public CatalogController(ILibraryAsset assets, ICheckOut checkouts)
        {
            _assets = assets;
            _checkouts = checkouts;
        }

        public IActionResult Index()
        {
            IEnumerable<LibraryAsset> assetModels = _assets.GetAll();


            IEnumerable<AssetIndexListingModel> listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.Id,
                    Title = result.Title,
                    AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                    ImageUrl = result.ImageUrl,
                    NumberOfCopies = result.NumberOfCopies.ToString(),
                    DeweyCallNumber = _assets.GetDeweyIndex(result.Id),
                    Type = _assets.GetType(result.Id)

                });

            AssetIndexModel model = new AssetIndexModel();
            model.Assets = listingResult;

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            LibraryAsset asset = _assets.GetById(id);

            IEnumerable<AssetHoldModel> currentHolds = _checkouts.GetCurrentHolds(asset.Id)
                                                       .Select(h => new AssetHoldModel
                                                       {
                                                           HoldPlaced = h.HoldPlaced.ToShortDateString(),
                                                           PatronName = _checkouts.GetCurrentHoldPatronName(h.Id)
                                                       });

            AssetDetailModel model = new AssetDetailModel()
            {
                AssetId = asset.Id,
                Title = asset.Title,
                Year = asset.Year,
                DeweyCallNumber = _assets.GetDeweyIndex(asset.Id),
                ISBN = _assets.GetIsbn(asset.Id),
                Type = _assets.GetType(asset.Id),
                ImageUrl = asset.ImageUrl,
                Cost = asset.Cost,
                AuthorOrDirector = _assets.GetAuthorOrDirector(asset.Id),
                CurrentLocation = _assets.GetCurrentLocation(asset.Id).Name,
                Status = asset.Status.Name,
                CheckoutHistory = _checkouts.GetCheckoutHistory(asset.Id),
                CurrentHolds = currentHolds,
                LatestCheckout = _checkouts.GetLatestCheckout(asset.Id),
                PatronName = _checkouts.GetCurrentCheckoutPatron(asset.Id),
            };

            return View(model);
        }

        public IActionResult CheckOut(int id)
        {
            LibraryAsset asset = _assets.GetById(id);

            CheckoutModel model = new CheckoutModel()
            {
                AssetId = asset.Id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                IsCheckedOut = _checkouts.IsCheckedOut(asset.Id),
                LibraryCardId = ""
            };

            return View(model);
        }

        public IActionResult CheckIn(int assetId)
        {
            _checkouts.CheckInItem(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }


        public IActionResult Hold(int id)
        {
            LibraryAsset asset = _assets.GetById(id);

            CheckoutModel model = new CheckoutModel()
            {
                AssetId = asset.Id,
                HoldCount = _checkouts.GetCurrentHolds(asset.Id).Count(),
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                IsCheckedOut = _checkouts.IsCheckedOut(asset.Id),
                LibraryCardId = ""
            };

            return View(model);
        }

        public IActionResult MarkLost(int assetId)
        {
            _checkouts.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult MarkFound(int assetId)
        {
            _checkouts.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            _checkouts.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkouts.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}
