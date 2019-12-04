namespace MusicHub.DataProcessor
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
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            var desWriters = JsonConvert.DeserializeObject<ImportWriterDTO[]>(jsonString);
            var validWriters = new List<Writer>();
            var sb = new StringBuilder();

            foreach (var writer in desWriters)
            {
                if (!IsValid(writer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var w = new Writer { Name = writer.Name, Pseudonym = writer.Pseudonym };
                validWriters.Add(w);
                sb.AppendLine(String.Format(SuccessfullyImportedWriter, writer.Name));
            }
            context.AddRange(validWriters);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var desPA = JsonConvert.DeserializeObject<ImportProducersAlbumsDTO[]>(jsonString);//, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            var validPA = new List<Producer>();
            var sb = new StringBuilder();

            foreach (var pa in desPA)
            {
                if (!IsValid(pa) || !pa.Albums.All(IsValid))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var producer = new Producer
                {
                    Name = pa.Name,
                    Pseudonym = pa.Pseudonym,
                    PhoneNumber = pa.PhoneNumber
                };

                foreach (var album in pa.Albums)
                {
                    var a = AutoMapper.Mapper.Map<Album>(album);
                    producer.Albums.Add(a);
                }
                validPA.Add(producer);

                if (pa.PhoneNumber != null)
                {
                    sb.AppendLine($"Imported {pa.Name} with phone: {pa.PhoneNumber} produces {pa.Albums.Count()} albums");
                }
                else
                {
                    sb.AppendLine($"Imported {pa.Name} with no phone number produces {pa.Albums.Count()} albums");
                }

            }
            context.AddRange(validPA);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSongDTO[]), new XmlRootAttribute("Songs"));

            var songsDTO = (ImportSongDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validSongs = new List<Song>();

            var sb = new StringBuilder();

            foreach (var song in songsDTO)
            {
                if (!IsValid(song))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var genre = Enum.TryParse(song.Genre, out Genre genreResult);
                var songName = validSongs.Any(s => s.Name == song.Name);
                var album = context.Albums.Find(song.AlbumId);
                var writer = context.Writers.Find(song.WriterId);

                if (!genre || album == null || writer == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var a = AutoMapper.Mapper.Map<Song>(song);
                validSongs.Add(a);
                sb.AppendLine(string.Format(SuccessfullyImportedSong, song.Name, song.Genre, song.Duration));

            }

            context.Songs.AddRange(validSongs);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSongPerformersDTO[]), new XmlRootAttribute("Performers"));

            var performersDTO = (ImportSongPerformersDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validPerformers = new List<Performer>();

            var songID = context.Songs.Select(s => s.Id).ToList();

            var sb = new StringBuilder();

            foreach (var performer in performersDTO)
            {
                if (!IsValid(performer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var validSongsCount = context.Songs.Count(s => performer.PerformersSongs.Any(i => i.SongId == s.Id));

                if (validSongsCount != performer.PerformersSongs.Length)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var a = AutoMapper.Mapper.Map<Performer>(performer);
                validPerformers.Add(a);
                sb.AppendLine(string.Format(SuccessfullyImportedPerformer, performer.FirstName, performer.PerformersSongs.Count()));
            }

            context.Performers.AddRange(validPerformers);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            var result = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return result;
        }
    }
}