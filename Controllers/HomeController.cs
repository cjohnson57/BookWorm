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
    public class HomeController : Controller
    {
        private DatabaseAPI api = new DatabaseAPI();

        //Basic page that displays each book and its related books
        public IActionResult Index()
        {
            return View(ConvertToRelated(api.GetAllBooks()));
        }

        //Basically the same as the index page but there's a search bar, so the model adds a query field
        public IActionResult Search()
        {
            SearchModel model = new SearchModel();
            model.Books = ConvertToRelated(api.GetAllBooks());
            model.Query = "";
            return View(model);
        }

        //If a search query was given, parse the search and return only relevant books
        [HttpPost]
        public IActionResult Search(string query)
        {
            SearchModel model = new SearchModel();
            if (string.IsNullOrEmpty(query)) //No search given, just get all books as normal
            {
                model.Books = ConvertToRelated(api.GetAllBooks());
            }
            else //A search was given, parse the serch
            {
                model.Books = ConvertToRelated(ParseSearch(query));
            }      
            model.Query = query; //Keep query in search bar when page reloads
            return View(model);
        }

        //Converts a list of Book objects to a list of BookAndRelated objects by getting some related books from the API
        private List<BookAndRelated> ConvertToRelated(List<Book> toconvert)
        {
            List<BookAndRelated> books = new List<BookAndRelated>();
            foreach (Book book in toconvert)
            {
                BookAndRelated b = new BookAndRelated(book);
                b.Related = GetRelatedBookNames(book); //Attach related book names to object
                books.Add(b);
            }
            return books;
        }

        //Gets a list of up to 3 names of books that are related to the given book
        private List<string> GetRelatedBookNames(Book book)
        {
            List<string> relatednames = new List<string>();
            List<Book> related = ShuffleBooks(api.GetRelatedBooks(book)).Take(3).ToList(); //Get up to 3 randomized, related books
            foreach (Book b in related)
            {
                relatednames.Add(b.Title);
            }
            return relatednames;
        }

        //Used to randomize order of related books
        public static List<Book> ShuffleBooks<Book>(List<Book> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Book value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        //Given a search string, parse it to figure out what the user is searching for and filter the list of Books based on this extracted query information
        private List<Book> ParseSearch(string query)
        {
            Dictionary<string, int> keys = new Dictionary<string, int>();
            //Inspect query to check for search keys for author, series, and rating fields
            int authorindex = query.IndexOf("author:", StringComparison.OrdinalIgnoreCase);
            if (authorindex > -1)
            {
                keys.Add("Author", authorindex);
            }
            int seriesindex = query.IndexOf("series:", StringComparison.OrdinalIgnoreCase);
            if (seriesindex > -1)
            {
                keys.Add("Series", seriesindex);
            }
            int ratingindex = query.IndexOf("rating:", StringComparison.OrdinalIgnoreCase);
            if (ratingindex > -1)
            {
                keys.Add("Rating", ratingindex);
            }
            keys = keys.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); //Order keys by index within the string
            List<Book> all = api.GetAllBooks(); //First get all books, and pare them down as search executes
            if (keys.Count() > 0) //There are search keys, must check them
            {
                //Title will always come first, if it exists
                string title = query.Substring(0, keys.ElementAt(0).Value).Trim();
                if (!string.IsNullOrEmpty(title))
                {
                    all = all.Where(x => x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                //Go through each key in order and parse their values, paring down the books to be returned
                for (int i = 0; i < keys.Count(); i++)
                {
                    int begin = keys.ElementAt(i).Value + 7; //Coincidentally the offset for "author:", "series:", and "rating:" are all 7
                    int end = i == keys.Count - 1 ? query.Length : keys.ElementAt(i + 1).Value; //If this is the last key then check until the end of the string, else end at start of next key
                    string value = query.Substring(begin, end - begin).Trim();
                    if (keys.ElementAt(i).Key == "Author")
                    {
                        all = all.Where(x => x.Author.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList(); //Get books with matching author
                    }
                    else if (keys.ElementAt(i).Key == "Series")
                    {
                        all = all.Where(x => x.Series != null && x.Series.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList(); //Get books with matching series
                    }
                    else if (keys.ElementAt(i).Key == "Rating")
                    {
                        //For rating, we must check if there is a range with 2 numbers separated by - (ex, 1-3) or just a single number
                        string[] scores = value.Split('-');
                        int lower, upper;
                        int.TryParse(scores[0], out lower); //Get lower, or only rating
                        if (scores.Length > 1) //There is a second number
                        {
                            int.TryParse(scores[1], out upper);
                        }
                        else //Else just set equal, so exact score will be checked
                        {
                            upper = lower;
                        }
                        if (lower > 0 && upper > 0) //Make sure both parsed correctly
                        {
                            all = all.Where(x => lower <= x.Score && x.Score <= upper).ToList(); //Get all books within score range
                        }
                    }
                }
            }
            else //No keys, just check title
            {
                all = all.Where(x => x.Title.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return all;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
