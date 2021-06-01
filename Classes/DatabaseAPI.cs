using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Data.SqlClient;
using BookWormSite.Models;

namespace BookWormSite.Classes
{
    class DatabaseAPI
    {
        private ISession session = NHibernateHelper.GetCurrentSession();

        //Simply get all Books from the DB
        public List<Book> GetAllBooks()
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                return (from book in session.Query<Book>() select book).ToList();
            }
        }

        //Get a specific Book given its primary key
        public Book GetBook(int pk)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                return (from book in session.Query<Book>() where book.BookPK == pk select book).FirstOrDefault();
            }
        }

        //Update each Book in the given list in the DB if anything has changed
        public void UpdateBooks(List<Book> books)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                foreach (Book book in books)
                {
                    session.Merge(book);
                }
                tx.Commit();
            }
        }

        //Add a Book object to the DB
        public bool AddBook(Book newbook)
        {
            try
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Save(newbook);
                    tx.Commit();
                }
                return true; //Indicate book saved successfully
            }
            catch (Exception ex)
            {
                return false; //Indicate some error occured
            }

        }

        //Delete a Book from the DB given its primary key
        public bool DeleteBook(int pk)
        {
            try
            {
                Book todelete = GetBook(pk);
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(todelete);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Delete a book from the DB given a Book object
        public bool DeleteBook(Book todelete)
        {
            try
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(todelete);
                    tx.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        //Get books with matching series or author to given book
        public List<Book> GetRelatedBooks(Book b)
        {
            using (ITransaction tx = session.BeginTransaction())
            {
                if (string.IsNullOrEmpty(b.Series))
                {
                    return (from book in session.Query<Book>() where book.Author == b.Author && book.BookPK != b.BookPK select book).ToList();
                }
                return (from book in session.Query<Book>() where (book.Author == b.Author || book.Series == b.Series) && book.BookPK != b.BookPK select book).ToList();
            }
        }
    }
}
