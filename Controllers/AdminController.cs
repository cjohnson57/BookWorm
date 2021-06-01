using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookWormSite.Models;
using BookWormSite.Classes;
using NHibernate;

namespace BookWormSite.Controllers
{
    //Controller for pages that can modify the database
    public class AdminController : Controller
    {
        private DatabaseAPI api = new DatabaseAPI();

        //A page listing all books in the DB
        public IActionResult EditBooks()
        {
            //Get all Books from the DB and convert them to BookPageForm for use on the page
            List<BookPageForm> model = new List<BookPageForm>();
            foreach (Book b in api.GetAllBooks())
            {
                model.Add(new BookPageForm(b));
            }
            return View(model);
        }

        //Saves any changes made to any Book in the DB
        [HttpPost]
        public IActionResult EditBooks(List<BookPageForm> submitdata)
        {
            //Convert each BookPageForm into a Book object
            List<Book> books = new List<Book>();
            foreach (BookPageForm b in submitdata)
            {
                books.Add(new Book(b, api.GetBook(b.BookPK)));
            }
            api.UpdateBooks(books); //Save any changes made
            return EditBooks();
        }

        //Make a new BookPageForm to be filled by the page
        public IActionResult AddBook()
        {
            return View(new BookPageForm());
        }

        //Attempt to save Book to database
        [HttpPost]
        public IActionResult AddBook(BookPageForm submitdata)
        {
            Book newbook = new Book(submitdata);
            if (api.AddBook(newbook)) //Attempt save
            {
                submitdata.BookPK = -1; //Indicates to page the Book saved successfully
            }
            else //Book failed to save
            {
                submitdata.BookPK = -2; //Indicates to page the Book failed to save
            }

            return View(submitdata);
        }

        //Page to display an existing book and confirm if user wants to delete
        public IActionResult DeleteBook(int DeleteKey)
        {
            return View(api.GetBook(DeleteKey));
        }

        //Attempt to delete an existing Book from the database
        [HttpPost]
        public IActionResult DeleteBook(Book submitdata)
        {
            if (api.DeleteBook(submitdata)) //Attempt delete
            {
                submitdata.BookPK = -1; //Indicates to page the Book deleted successfully
            }
            else //Book failed to delete
            {
                submitdata.BookPK = -2; //Indicates to page the Book failed to delete
            }
            return View(submitdata);
        }

    }
}
