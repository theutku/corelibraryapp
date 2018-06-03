using System.Collections.Generic;
using System.Linq;
using Library.Models.BranchModels;
using LibraryData.Interfaces;
using LibraryData.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class BranchController : Controller
    {
        private readonly ILibraryBranch _branches;

        public BranchController(ILibraryBranch branches)
        {
            _branches = branches;
        }

        public IActionResult Index()
        {
            IEnumerable<BranchDetailModel> branches = _branches.GetAll().Select(lb => new BranchDetailModel
            {
                Id = lb.Id,
                BranchName = lb.Name,
                IsOpen = _branches.IsBranchOpen(lb.Id) ? "Open" : "Closed",
                NumberOfAssets = _branches.GetAssetCount(lb.Id),
                NumberOfPatrons = _branches.GetPatronCount(lb.Id)
            });

            BranchIndexModel model = new BranchIndexModel()
            {
                Branches = branches
            };

            return View(model);
        }

        public IActionResult Detail(int branchId)
        {
            LibraryBranch branch = _branches.GetLibraryBranch(branchId);

            BranchDetailModel model = new BranchDetailModel()
            {
                Id = branch.Id,
                BranchName = branch.Name,
                Address = branch.Address,
                Description = branch.Description,
                Telephone = branch.Telephone,
                BranchOpenedDate = branch.OpenDate.ToString("yyyy-MM-dd"),
                ImageUrl = branch.ImageUrl,
                IsOpen = _branches.IsBranchOpen(branch.Id) ? "Open" : "Closed",
                HoursOpen = _branches.GetBranchHours(branch.Id),
                NumberOfAssets = _branches.GetAssetCount(branch.Id),
                NumberOfPatrons = _branches.GetPatronCount(branch.Id),
                TotalAssetValue = _branches.GetAssetsValue(branch.Id)
            };

            return View(model);
        }
    }
}
