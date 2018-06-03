using System;
using System.Collections.Generic;
using LibraryData;
using LibraryData.Interfaces;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private readonly LibraryContext _context;

        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset newAsset)
        {
            this._context.LibraryAssets.Add(newAsset);
            this._context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return this._context.LibraryAssets
                       .Include(asset => asset.Status)
                       .Include(asset => asset.Location);
        }

        public string GetAuthorOrDirector(int id)
        {
            bool isBook = this._context.LibraryAssets.OfType<Book>().Where(item => item.Id == id).Any();
            bool isVideo = this._context.LibraryAssets.OfType<Video>().Where(item => item.Id == id).Any();

            return isBook ?
                this._context.Books.FirstOrDefault(item => item.Id == id).Author : this._context.Videos.FirstOrDefault(item => item.Id == id).Director
                    ?? "Unknown";
        }

        public LibraryAsset GetById(int id)
        {
            return this._context.LibraryAssets
                       .Include(asset => asset.Status)
                       .Include(asset => asset.Location)
                       .FirstOrDefault(asset => asset.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (this._context.Books.Any(item => item.Id == id))
            {
                return this._context.Books.Find(id).DeweyIndex;
            }

            return "";
        }

        public string GetIsbn(int id)
        {
            if (this._context.Books.Any(item => item.Id == id))
            {
                return this._context.Books.Find(id).ISBN;
            }

            return "";
        }

        public string GetTitle(int id)
        {
            return GetById(id).Title;
        }

        public string GetType(int id)
        {
            var book = this._context.LibraryAssets.OfType<Book>().Where(item => item.Id == id);

            return book.Any() ? "Book" : "Video";
        }
    }
}
