using System;
using System.Collections.Generic;
using System.Linq;
using LibraryData;
using LibraryData.Interfaces;
using LibraryData.Models;
using LibraryServices.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class LibraryBranchService : ILibraryBranch
    {
        private readonly LibraryContext _context;

        public LibraryBranchService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryBranch newBranch)
        {
            _context.LibraryBranches.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch GetLibraryBranch(int branchId)
        {
            return _context.LibraryBranches
                           .Include(lb => lb.LibraryAssets)
                           .Include(lb => lb.Patrons)
                           .FirstOrDefault(lb => lb.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                           .Include(lb => lb.LibraryAssets)
                           .Include(lb => lb.Patrons);
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return GetLibraryBranch(branchId).LibraryAssets;
        }


        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return GetLibraryBranch(branchId).Patrons;
        }


        public IEnumerable<string> GetBranchHours(int branchId)
        {
            IEnumerable<BranchHours> branchHours = GetRawBranchHours(branchId);

            return DataHelpers.HumanizeBusinessHours(branchHours);
        }

        public bool IsBranchOpen(int branchId)
        {
            int currentTimeHour = DateTime.Now.Hour;
            int currentDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;

            IEnumerable<BranchHours> branchHours = GetRawBranchHours(branchId);

            var daysHours = branchHours.FirstOrDefault(h => h.DayOfWeek == currentDayOfWeek);

            return currentTimeHour < daysHours.CloseTime && currentTimeHour > daysHours.OpenTime;
        }

        public int GetAssetCount(int branchId)
        {
            return GetAssets(branchId).Count();
        }

        public int GetPatronCount(int branchId)
        {
            return GetPatrons(branchId).Count();
        }

        public decimal GetAssetsValue(int id)
        {
            var assetsValue = GetAssets(id).Select(a => a.Cost);
            return assetsValue.Sum();
        }

        private IEnumerable<BranchHours> GetRawBranchHours(int branchId)
        {
            return _context.BranchHours
                           .Where(bh => bh.Branch.Id == branchId).ToList();
        }
    }
}
