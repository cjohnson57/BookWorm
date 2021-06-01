using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace BookWormSite.Models
{
    //Contains a list of Books to display and a query used to determine the Book list
    public class SearchModel
    {
        public string Query;
        public List<BookAndRelated> Books;
    }
}
