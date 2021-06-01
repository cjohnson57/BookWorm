using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using BookWormSite.Models;

namespace BookWormSite.Classes
{
    //Used by NHibernate to map Book objects from the DB
    public class BookMapping : ClassMap<Book>
    {
        public BookMapping()
        {
            Table("Books");
            Id(x => x.BookPK).GeneratedBy.Increment(); //Increment the PK when a new row is added
            Map(x => x.Title);
            Map(x => x.Author);
            Map(x => x.Synopsis);
            Map(x => x.Series);
            Map(x => x.Review);
            Map(x => x.Score);
            Map(x => x.Cover).Length(int.MaxValue); //Specify length as maximum possible value
        }
    }

}
