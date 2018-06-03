using System;
using System.Collections.Generic;
using LibraryData.Models;

namespace LibraryData.Interfaces
{
    public interface ICheckOut
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int checkoutId);
        Checkout GetLatestCheckout(int assetId);

        void Add(Checkout newCheckout);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId);
        void PlaceHold(int assetId, int libraryCardId);
        void MarkLost(int assetId);
        void MarkFound(int assetId);

        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        string GetCurrentCheckoutPatron(int assetId);

        string GetCurrentHoldPatronName(int holdId);
        DateTime GetCurrentHoldPlaced(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        bool IsCheckedOut(int assetId);
    }
}
