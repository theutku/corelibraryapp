using System.Collections.Generic;
using LibraryData.Models;

namespace LibraryData.Interfaces
{
    public interface IPatron
    {
        Patron GetPatron(int id);
        IEnumerable<Patron> GetAll();
        void Add(Patron newPatron);

        IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId);
        IEnumerable<Hold> GetHolds(int patronId);
        IEnumerable<Checkout> GetCheckouts(int patronId);

    }
}
