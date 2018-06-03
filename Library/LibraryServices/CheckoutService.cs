using System;
using System.Collections.Generic;
using System.Linq;
using LibraryData;
using LibraryData.Interfaces;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class CheckoutService : ICheckOut
    {
        private readonly LibraryContext _context;

        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Checkout newCheckout)
        {
            _context.Checkouts.Add(newCheckout);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            LibraryAsset item = _context.LibraryAssets.FirstOrDefault(asset => asset.Id == assetId);

            RemoveExistingCheckouts(assetId);
            CloseExistingCheckoutHistory(assetId);

            IEnumerable<Hold> currentHolds = _context.Holds
                                                     .Include(h => h.LibraryCard)
                                                     .Include(h => h.LibraryAsset)
                                                     .Where(h => h.LibraryAsset.Id == assetId);
            if (currentHolds.Any())
            {
                CheckoutToEarlistHold(assetId, currentHolds);
                return;
            }

            UpdateAssetStatus(assetId, "Available");
            _context.SaveChanges();
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId)) return;

            LibraryAsset item = _context.LibraryAssets.FirstOrDefault(asset => asset.Id == assetId);

            UpdateAssetStatus(assetId, "Checked Out");

            LibraryCard libraryCard = _context.LibraryCards
                                              .Include(card => card.Checkouts)
                                              .FirstOrDefault(card => card.Id == libraryCardId);

            DateTime now = DateTime.Now;

            Checkout checkout = new Checkout()
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultcheckoutTime(now)
            };

            _context.Checkouts.Add(checkout);

            CheckoutHistory checkoutHistory = new CheckoutHistory()
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.CheckoutHistories.Add(checkoutHistory);
            _context.SaveChanges();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return _context.Checkouts.Find(checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                           .Include(item => item.LibraryAsset)
                           .Include(item => item.LibraryCard)
                           .Where(item => item.LibraryAsset.Id == id);
        }

        public string GetCurrentHoldPatronName(int holdId)
        {
            Hold currentHold = _context.Holds
                                       .Include(h => h.LibraryCard)
                                       .Include(h => h.LibraryAsset)
                                       .FirstOrDefault(h => h.Id == holdId);

            if (currentHold != null)
            {
                var cardId = currentHold.LibraryCard.Id;
                return GetPatronByCard(cardId);
            }

            return "";

        }

        public DateTime GetCurrentHoldPlaced(int id)
        {
            return _context.Holds
                           .Include(h => h.LibraryCard)
                           .Include(h => h.LibraryAsset)
                           .FirstOrDefault(h => h.Id == id)
                           .HoldPlaced;
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                           .Include(item => item.LibraryAsset)
                           .Where(item => item.LibraryAsset.Id == id);
        }

        public Checkout GetLatestCheckout(int assetId)
        {
            return _context.Checkouts
                           .Where(item => item.LibraryAsset.Id == assetId)
                           .OrderByDescending(item => item.Since)
                           .FirstOrDefault();
        }

        public void MarkFound(int assetId)
        {

            UpdateAssetStatus(assetId, "Available");
            RemoveExistingCheckouts(assetId);
            CloseExistingCheckoutHistory(assetId);

            _context.SaveChanges();
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");
            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            LibraryAsset item = _context.LibraryAssets
                                        .Include(asset => asset.Status)
                                        .FirstOrDefault(asset => asset.Id == assetId);
            LibraryCard card = _context.LibraryCards.FirstOrDefault(lc => lc.Id == libraryCardId);

            if (item.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            Hold newHold = new Hold()
            {
                LibraryAsset = item,
                LibraryCard = card,
                HoldPlaced = DateTime.Now
            };

            _context.Holds.Add(newHold);
            _context.SaveChanges();
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            Checkout checkOut = GetCheckoutByAssetId(assetId);

            if (checkOut == null)
            {
                return "";
            }

            int cardId = checkOut.LibraryCard.Id;

            return GetPatronByCard(cardId);
        }

        public bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts.Where(co => co.LibraryAsset.Id == assetId).Any();
        }

        private string GetPatronByCard(int libraryCardId)
        {
            Patron currentPatron = _context.Patrons
                                          .Include(p => p.LibraryCard)
                                           .FirstOrDefault(p => p.LibraryCard.Id == libraryCardId);

            if (currentPatron != null)
                return currentPatron.FirstName + " " + currentPatron.LastName;
            return "";
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                            .Include(co => co.LibraryCard)
                            .Include(co => co.LibraryAsset)
                            .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
        }


        private void UpdateAssetStatus(int assetId, string status)
        {
            LibraryAsset item = _context.LibraryAssets.FirstOrDefault(asset => asset.Id == assetId);

            _context.Update(item);
            item.Status = _context.Statuses.FirstOrDefault(stat => stat.Name == status);
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            Checkout previousCheckout = _context.Checkouts.FirstOrDefault(co => co.LibraryAsset.Id == assetId);

            if (previousCheckout != null)
            {
                _context.Remove(previousCheckout);
                _context.SaveChanges();
            }
        }

        private void CloseExistingCheckoutHistory(int assetId)
        {
            CheckoutHistory history = _context.CheckoutHistories.FirstOrDefault(coh => coh.LibraryAsset.Id == assetId && coh.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = DateTime.Now;
            }
        }

        private void CheckoutToEarlistHold(int assetId, IEnumerable<Hold> currentHolds)
        {
            Hold earliestHold = currentHolds.OrderBy(item => item.HoldPlaced).FirstOrDefault();
            if (earliestHold == null) return;
            LibraryCard card = earliestHold.LibraryCard;

            _context.Remove(earliestHold);
            _context.SaveChanges();

            CheckOutItem(assetId, card.Id);
        }

        private DateTime GetDefaultcheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }


    }
}
