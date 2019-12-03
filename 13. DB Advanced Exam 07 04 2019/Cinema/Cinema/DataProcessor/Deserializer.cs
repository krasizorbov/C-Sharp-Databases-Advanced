namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var desMovies = JsonConvert.DeserializeObject<Movie[]>(jsonString);

            var validMovies = new List<Movie>();

            var sb = new StringBuilder();

            foreach (var m in desMovies)
            {
                var isValidEnum = Enum.TryParse(typeof(Genre), m.Genre.ToString(), out object result);
                var movieExists = validMovies.Any(t => t.Title == m.Title);

                if (!IsValid(m) || !isValidEnum || movieExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movie = new Movie { Title = m.Title, Genre = m.Genre, Duration = m.Duration, Rating = m.Rating, Director = m.Director };
                validMovies.Add(movie);

                sb.AppendLine(String.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("F2")));
            }

            context.AddRange(validMovies);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var desHalls = JsonConvert.DeserializeObject<ImportHallSeats[]>(jsonString);

            var validHalls = new List<Hall>();

            var sb = new StringBuilder();

            foreach (var h in desHalls)
            {

                if (!IsValid(h))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = new Hall { Name = h.Name, Is4Dx = h.Is4Dx, Is3D = h.Is3D };

                for (int i = 0; i < h.Seats; i++)
                {
                    hall.Seats.Add(new Seat());
                }
                validHalls.Add(hall);

                var status = string.Empty;

                if (hall.Is4Dx && hall.Is3D)
                {
                    status = "4Dx/3D";
                }
                else if(hall.Is4Dx && !hall.Is3D)
                {
                    status = "4Dx";
                }
                else if (!hall.Is4Dx && hall.Is3D)
                {
                    status = "3D";
                }
                else
                {
                    status = "Normal";
                }

                sb.AppendLine(String.Format(SuccessfulImportHallSeat, hall.Name, status, hall.Seats.Count));
            }

            context.AddRange(validHalls);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectionsDTO[]), new XmlRootAttribute("Projections"));

            var projectionsDTO = (ImportProjectionsDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validProjections = new List<Projection>();

            var sb = new StringBuilder();

            foreach (var projection in projectionsDTO)
            {
                var movies = context.Movies.Find(projection.MovieId);
                var halls = context.Halls.Find(projection.HallId);

                if (movies == null || halls == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var p = new Projection
                {
                    MovieId = projection.MovieId,
                    HallId = projection.HallId,
                    DateTime = DateTime.ParseExact(projection.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };
                validProjections.Add(p);
                sb.AppendLine(string.Format(SuccessfulImportProjection, movies.Title, p.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
            }

            context.Projections.AddRange(validProjections);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerTicketDTO[]), new XmlRootAttribute("Customers"));

            var customersDTO = (ImportCustomerTicketDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validCustomers = new List<Customer>();

            var sb = new StringBuilder();

            foreach (var customer in customersDTO)
            {
                var projectionsID = context.Projections.Select(x => x.Id).ToArray();
                var invalidProjections = projectionsID.Any(p => customer.Tickets.Any(s => s.ProjectionId != p));

                if (!IsValid(customer) && customer.Tickets.All(IsValid) && invalidProjections)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var c = new Customer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Age = customer.Age,
                    Balance = customer.Balance
                };

                foreach (var ticket in customer.Tickets)
                {
                    c.Tickets.Add(new Ticket
                    {
                        ProjectionId = ticket.ProjectionId,
                        Price = ticket.Price
                    });
                }

                validCustomers.Add(c);

                sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count()));
            }

            context.Customers.AddRange(validCustomers);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);

            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, results, true);
        }
    }
}