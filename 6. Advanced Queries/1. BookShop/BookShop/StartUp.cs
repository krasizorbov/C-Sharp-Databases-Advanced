namespace BookShop
{
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);
            }
        }
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(b => b.Copies < 4200).ToList();

            context.Books.RemoveRange(books);

            context.SaveChanges();

            return books.Count();
        }
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var books = context.Categories.Select(c => new
            {
                categoryName = c.Name,
                bookTitle = c.CategoryBooks.Select(cb => new
                {
                    bookTitle = cb.Book.Title,
                    releaseDate = cb.Book.ReleaseDate
                })
                .OrderByDescending(r => r.releaseDate)
                .Take(3)
                .ToList()
            })
                .OrderBy(e => e.categoryName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var category in books)
            {
                sb.AppendLine($"--{category.categoryName}");

                foreach (var b in category.bookTitle)
                {
                    sb.AppendLine($"{b.bookTitle} ({b.releaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var bookProfit = context.Categories.Select(x => new
            {
                categoryName = x.Name,
                totalPrice = x.CategoryBooks.Sum(e => e.Book.Price * e.Book.Copies)
            })
                .OrderByDescending(t => t.totalPrice)
                .ThenBy(t => t.categoryName)
                .ToList();

            var result = string.Join(Environment.NewLine, bookProfit.Select(x => $"{x.categoryName} ${x.totalPrice:F2}"));

            return result;
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var books = context.Authors.Select(x => new
            {
                firstName = x.FirstName,
                lastName = x.LastName,
                sum = x.Books.Sum(b => b.Copies)
            })
                .OrderByDescending(c => c.sum)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.firstName} {x.lastName} - {x.sum}"));

            return result;
        }
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books.Where(b => b.Title.Length > lengthCheck).Count();
        }
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books.Where(a => EF.Functions.Like(a.Author.LastName, input + "%"))
                .Select(b => new
                {
                    bookTitle = b.Title,
                    authorFirstName = b.Author.FirstName,
                    authorLastName = b.Author.LastName
                })
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.bookTitle} ({x.authorFirstName} {x.authorLastName})"));

            return result;
        }
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books.Where(b => EF.Functions.Like(b.Title, "%" + input + "%"))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors.Where(a => a.FirstName.EndsWith(input))
                .Select(n => $"{n.FirstName} {n.LastName}")
                .OrderBy(n => n)
                .ToList();

            var result = string.Join(Environment.NewLine, authors);

            return result;
        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateConversion = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books.Where(b => b.ReleaseDate.Value < dateConversion)
                .OrderByDescending(b => b.ReleaseDate.Value)
                .Select(b => new
                {
                    bookTitle = b.Title,
                    bookEditionType = b.EditionType,
                    bookPrice = b.Price
                })
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.bookTitle} - {x.bookEditionType} - ${x.bookPrice:F2}"));

            return result;
        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

            var books = context.Books
                .Where(b => b.BookCategories
                .Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(t => t.Title)
                .OrderBy(t => t)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books.Where(d => d.ReleaseDate.Value.Year != year)
                .Select(b => b.Title).ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books.Where(x => x.Price > 40)
                .Select(b => new
                {
                    bookTitle = b.Title,
                    bookPrice = b.Price
                })
                .OrderByDescending(x => x.bookPrice)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.bookTitle} - ${x.bookPrice:F2}"));

            return result;
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books.Where(x => x.EditionType.ToString() == "Gold" && x.Copies < 5000)
                .Select(x => x.Title).ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var books = context.Books.Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(x => x.Title).OrderBy(x => x).ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
    }
}
