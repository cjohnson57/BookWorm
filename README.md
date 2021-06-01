# BookWorm
This is a simple site which connectes to a Books table that stores information about books in a library.

[CreateDatabase.sql](CreateDatabase.sql) is a script to create the BookWorm database and Books table, as well as insert some sample data.
Since book covers are stored as binary data within the database, this script is quite large.

By default, the site connects to a database BookWorm hosted on localhost. To change this, edit the ConnectionString property on line 10 of [appsettings.json](appsettings.json).

The main page of the site simply lists all books within the database, with each having its own dropdown panel that displays information such as title, author,
a synopsis, score from 1 to 5 stars, and (if present) a series, the cover, and up to three related books. Related books are defined as books by the same author, or
within the same series.

The second page is a search page. Books are displayed the same here as on the main page, but filtered by the search bar. The search functions in the following manner:

By default anything entered in the search bar will search the title. You can also enter the following search keys in any order:
* `author:` - Text after the colon will search by author
* `series:` - Text after the colon will search by series
* `rating:` - Will search books with a rating number given after the colon. Can also provide a range (x-y). If the text after the colon isn't a number or a range,
this key will just be ignored.

Anything entered before a search key will also search the title. So, for example, the following search:
> return of the king series:lord of the rings author:tolkien rating:3-5

Will search for books that contain 'return of the king' in the title, in a series which contains the text 'lord of the rings', by an author whose name contains 'tolkien',
and with a rating in the range of 3 to 5 stars. The home and search pages are handled in [HomeController.cs](Controllers/HomeController.cs)

Finally, there is a manage page, which lists each book's properties in text fields to allow you to edit them, as well as upload book covers.
This page also links to a page to add a new book, and a page to delete any book. These pages are all handled in [AdminController.cs](Controllers/AdminController.cs).

Modifications to the database are handled in [DatabaseAPI.cs](Classes/DatabaseAPI.cs), which uses [NHibernateHelper.cs](Classes/NHibernateHelper.cs) to connect to the database.
