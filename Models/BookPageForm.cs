using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookWormSite.Models
{
    //This is the same as the Book class, except Cover is an IFormFile for retrieving images from the frontend, and tags are added for data validation
    public class BookPageForm
    {
        public int BookPK { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Synopsis { get; set; }
        public string Series { get; set; }
        public string Review { get; set; }
        [Required]
        [Range(1, 5)]
        public int Score { get; set; }
        public IFormFile Cover { get; set; } //Cover as an IFormFile so it can be uploaded through the page

        public BookPageForm() 
        {
            Score = 1;
        }

        //Copy all properties from a Book
        public BookPageForm(Book b)
        {
            BookPK = b.BookPK;
            Title = b.Title;
            Author = b.Author;
            Synopsis = b.Synopsis;
            Series = b.Series;
            Review = b.Review;
            Score = b.Score;
        }
    }
}
