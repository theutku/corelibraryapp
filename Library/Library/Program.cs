using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibraryData;
using LibraryData.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (LibraryContext db = new LibraryContext())
            {
                db.Database.EnsureCreated();
            }

            BuildWebHost(args).Run();


        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

    }
}
