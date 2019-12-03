namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies.Where(x => x.Rating >= rating && x.Projections.Any(p => p.Tickets.Count >= 1))
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(p => p.Projections.Sum(t => t.Tickets.Sum(pc => pc.Price)))
                .Select(s => new
                {
                    MovieName = s.Title,
                    Rating = s.Rating.ToString("F2"),
                    TotalIncomes = s.Projections.Sum(d => d.Tickets.Sum(t => t.Price)).ToString("F2"),
                    Customers = s.Projections.SelectMany(c => c.Tickets).Select(m => new 
                    {
                        FirstName = m.Customer.FirstName,
                        LastName = m.Customer.LastName,
                        Balance = m.Customer.Balance.ToString("F2")
                    })
                    .OrderByDescending(b => b.Balance)
                    .ThenBy(f => f.FirstName)
                    .ThenBy(l => l.LastName)
                })
                .Take(10)
                .ToList();

            var serMovies = JsonConvert.SerializeObject(movies, new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });

            return serMovies;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers.Where(s => s.Age >= age)
                .Select(s => new ExportCustomerDTO
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    SpentMoney = s.Tickets.Sum(x => x.Price).ToString("F2"),
                    SpentTime = TimeSpan.FromSeconds(s.Tickets.Sum(x => x.Projection.Movie.Duration.TotalSeconds)).ToString(@"hh\:mm\:ss")
                })
                .OrderByDescending(p => decimal.Parse(p.SpentMoney))
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCustomerDTO[]), new XmlRootAttribute("Customers"));

            var sb = new StringBuilder();
            
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}