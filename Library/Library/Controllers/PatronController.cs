using System.Collections.Generic;
using System.Linq;
using Library.Models.PatronModels;
using LibraryData.Interfaces;
using LibraryData.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class PatronController : Controller
    {

        private readonly IPatron _patrons;

        public PatronController(IPatron patrons)
        {
            _patrons = patrons;
        }

        public IActionResult Index()
        {
            IEnumerable<PatronDetailModel> allPatrons = _patrons.GetAll().Select(p => new PatronDetailModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                HomeLibraryBranch = p.HomeLibraryBranch.Name,
                LibraryCardId = p.LibraryCard.Id,
                OverdueFees = p.LibraryCard.Fees,
            }).ToList();

            PatronIndexModel model = new PatronIndexModel
            {
                Patrons = allPatrons
            };

            return View(model);
        }

        public IActionResult Detail(int patronId)
        {
            Patron currentPatron = _patrons.GetPatron(patronId);

            PatronDetailModel model = new PatronDetailModel
            {
                Id = currentPatron.Id,
                FirstName = currentPatron.FirstName,
                LastName = currentPatron.LastName,
                Address = currentPatron.Address,
                Telephone = currentPatron.TelephoneNumber,
                HomeLibraryBranch = currentPatron.HomeLibraryBranch.Name,
                MemberSince = currentPatron.LibraryCard.Created,
                LibraryCardId = currentPatron.LibraryCard.Id,
                OverdueFees = currentPatron.LibraryCard.Fees,
                AssetsCheckedOut = _patrons.GetCheckouts(currentPatron.Id),
                CheckoutHistory = _patrons.GetCheckoutHistory(currentPatron.Id),
                Holds = _patrons.GetHolds(currentPatron.Id)
            };

            return View(model);
        }
    }
}
