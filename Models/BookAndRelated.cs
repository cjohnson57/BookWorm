using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace BookWormSite.Models
{
    //The same as the book class, but with an string list property for related books
    public class BookAndRelated : Book
    {
        public List<string> Related { get; set; }

        public BookAndRelated()
        {
            Score = 1;
        }

        //Copy all properties from Book object
        public BookAndRelated(Book b) : base(b)
        { }
    }
}
