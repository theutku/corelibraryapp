# DotNet Core 2.0 Library App w/ MySQL

A simple Library Application, using DotNet Core 2.0 MVC and Bootstrap.

The app utilizes MySQL as the relational database and uses dependency injection to perform database operations.

# How To Set Up & Run

  1. Simply insert your connection string in ```appsettings.json```, ```appsettings.Development.json``` inside Library.Web project folder & in ```LibraryContext.cs``` in Library.Data project folder.
  2. Hit Start (F5) and the database is created automatically.
  3. Execute the SQL commands DataSample.sql inside Library.Data project folder and you are all set!

### Objects

The application consists of the following objects:

| Object | Description |
| ------ | ------ |
| ```Patron``` | The library user |
| ```LibraryCard``` | Contains the information about a specific patron.  |
| ```LibraryAsset``` | Represents an item inside the library. Can be a book or a video. |
| ```Book``` | Represents a LibraryAsset of type Book. |
| ```Video``` | Represents a LibraryAsset of type Video. |
| ```Checkout``` | The loan record of a LibraryAsset. |
| ```CheckoutHistory``` | The loan and return history of a LibraryAsset. |
| ```Status``` | The current status of a LibraryAsset. Can be 'Available', 'Checked Out', 'On Hold' or 'Lost'. |
| ```Hold``` | The reservation status of a LibraryAsset. |
| ```BranchHours``` | The weekly opening and closing times of Libraries. |

### Operations

The application consists of the folowing operations:

* Check out / Check in a LibraryAsset
* Place a hold on a Library Asset.
* View Check Out Histories of LibraryAssets.
* View Holds placed on LibraryAssets.
* View Patrons' detailed informations.
* View Libraries' detailed informations.
