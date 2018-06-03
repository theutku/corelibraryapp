using System.Collections.Generic;
using System.Linq;
using LibraryData;
using LibraryData.Interfaces;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class PatronService : IPatron
    {
        private readonly LibraryContext _context;

        public PatronService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Patron newPatron)
        {
            _context.Patrons.Add(newPatron);
            _context.SaveChanges();
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            int cardId = GetPatron(patronId).LibraryCard.Id;

            return _context.CheckoutHistories
                           .Include(coh => coh.LibraryCard)
                           .Include(coh => coh.LibraryAsset)
                           .Where(coh => coh.LibraryCard.Id == cardId)
                           .OrderByDescending(coh => coh.CheckedOut);
        }

        public IEnumerable<Checkout> GetCheckouts(int patronId)
        {
            int cardId = GetPatron(patronId).LibraryCard.Id;

            return _context.Checkouts
                           .Include(co => co.LibraryCard)
                           .Include(co => co.LibraryAsset)
                           .Where(co => co.LibraryCard.Id == cardId);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            int cardId = GetPatron(patronId).LibraryCard.Id;

            return _context.Holds
                           .Include(h => h.LibraryCard)
                           .Include(h => h.LibraryAsset)
                           .Where(h => h.LibraryCard.Id == cardId)
                           .OrderByDescending(h => h.HoldPlaced);
        }

        public Patron GetPatron(int id)
        {
            return _context.Patrons
                           .Include(p => p.LibraryCard)
                           .Include(p => p.HomeLibraryBranch)
                           .FirstOrDefault(p => p.Id == id);
        }
    }
}
