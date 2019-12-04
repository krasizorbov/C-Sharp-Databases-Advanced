namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var games = JsonConvert.DeserializeObject<ImportGameDTO[]>(jsonString);

            var validGames = new List<Game>();

            var sb = new StringBuilder();

            foreach (var game in games)
            {
                if (!IsValid(game) || game.Tags.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var g = new Game { Name = game.Name, Price = game.Price, ReleaseDate = DateTime.ParseExact(game.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) };

                var developer = GetDeveloper(context, game.Developer);
                var genre = GetGenre(context, game.Genre);

                g.Developer = developer;
                g.Genre = genre;

                foreach (var t in game.Tags)
                {
                    var tag = GetTag(context, t);
                    g.GameTags.Add(new GameTag { Tag = tag });
                }

                validGames.Add(g);

                sb.AppendLine($"Added {g.Name} ({g.Genre.Name}) with {g.GameTags.Count} tags");
            }
            context.Games.AddRange(validGames);

            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        private static Tag GetTag(VaporStoreDbContext context, string t)
        {
            Tag tag = context.Tags.FirstOrDefault(x => x.Name == t);

            if (tag == null)
            {
                tag = new Tag { Name = t };
                context.Tags.Add(tag);
            }

            context.SaveChanges();
            return tag;
        }

        private static Genre GetGenre(VaporStoreDbContext context, string genre)
        {
            Genre gen = context.Genres.FirstOrDefault(x => x.Name == genre);

            if (gen == null)
            {
                gen = new Genre { Name = genre };
                context.Genres.Add(gen);
            }

            context.SaveChanges();
            return gen;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, string developer)
        {
            var dev = context.Developers.FirstOrDefault(x => x.Name == developer);

            if (dev == null)
            {
                dev = new Developer { Name = developer };
                context.Developers.Add(dev);
            }

            context.SaveChanges();
            return dev;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedUsers = JsonConvert.DeserializeObject<ImportUserDTO[]>(jsonString);

            var users = new List<User>();

            foreach (var userDto in deserializedUsers)
            {
                if (!IsValid(userDto) || !userDto.Cards.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var cards = userDto.Cards
                    .Select(c => new Card { Number = c.Number, Cvc = c.CVC, Type = c.Type })
                    .ToArray();

                var user = new User {
                    Username = userDto.Username,
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Age = userDto.Age,
                    Cards = cards
                };

                users.Add(user);
                sb.AppendLine($"Imported {userDto.Username} with {userDto.Cards.Length} cards");
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPurchasesDTO[]), new XmlRootAttribute("Purchases"));

            var purchasesDTO = (ImportPurchasesDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validPurchases = new List<Purchase>();

            var sb = new StringBuilder();

            foreach (var purchase in purchasesDTO)
            {
                var isValidEnum = Enum.TryParse<PurchaseType>(purchase.Type.ToString() ,out PurchaseType result);

                var card = context.Cards.Include(s => s.User).FirstOrDefault(x => x.Number == purchase.Card);

                var game = context.Games.FirstOrDefault(x => x.Name == purchase.Title);

                if (IsValid(purchase) && game != null && card != null && isValidEnum)
                {
                    var p = new Purchase
                    {
                        Type = purchase.Type,
                        ProductKey = purchase.Key,
                        Date = DateTime.ParseExact(purchase.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                        Card = card,
                        Game = game
                    };
                    validPurchases.Add(p);
                    sb.AppendLine($"Imported {p.Game.Name} for {p.Card.User.Username}");
                }
                else
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
            }

            context.Purchases.AddRange(validPurchases);

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