using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace BookWormSite.Models
{
    public class Book
    {
        //Matches properties in database
        public virtual int BookPK { get; set; }
        public virtual string Title { get; set; }
        public virtual string Author { get; set; }
        public virtual string Synopsis { get; set; }
        public virtual string Series { get; set; }
        public virtual string Review { get; set; }
        public virtual int Score { get; set; }
        public virtual byte[] Cover { get; set; }

        public Book()
        {
            Score = 1;
        }

        //Transfer all properties from one book to another
        public Book(Book b)
        {
            BookPK = b.BookPK;
            Title = b.Title;
            Author = b.Author;
            Synopsis = b.Synopsis;
            Series = b.Series;
            Review = b.Review;
            Score = b.Score;
            Cover = b.Cover;
        }

        //Get properties from BookPageForm, used when submitting from page
        public Book(BookPageForm b)
        {
            //Set similar properties
            BookPK = b.BookPK;
            Title = b.Title;
            Author = b.Author;
            Synopsis = b.Synopsis;
            Series = b.Series;
            Review = b.Review;
            Score = b.Score;
            //Cover was set, convert to bytes
            if (b.Cover != null && b.Cover.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    b.Cover.CopyTo(ms);
                    Cover = ms.ToArray();
                }
            }
        }

        //Original book given, to ensure cover not cleared if not necessary
        public Book(BookPageForm b, Book original)
        {
            //Set similar properties
            BookPK = b.BookPK;
            Title = b.Title;
            Author = b.Author;
            Synopsis = b.Synopsis;
            Series = b.Series;
            Review = b.Review;
            Score = b.Score;
            //Cover was set, convert to bytes
            if (b.Cover != null && b.Cover.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    b.Cover.CopyTo(ms);
                    Cover = ms.ToArray();
                }
            }
            //We don't want to overwrite the cover if it was not set on the edit page at all
            else if (original.Cover != null)
            {
                Cover = original.Cover;
            }
        }
    }
}
