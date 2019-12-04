namespace MusicHub.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using MusicHub.DataProcessor.Export_Dtos;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums.Where(x => x.ProducerId == producerId)
                .Select(s => new
                {
                    AlbumName = s.Name,
                    ReleaseDate = s.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = s.Producer.Name,
                    Songs = s.Songs.Select(x => new
                        {
                            SongName = x.Name,
                            Price = x.Price.ToString("F2"),
                            Writer = x.Writer.Name
                        })
                        .OrderByDescending(n => n.SongName)
                        .ThenBy(f => f.Writer)
                        .ToArray(),
                    AlbumPrice = s.Price.ToString("F2")
                })
                .OrderByDescending(p => decimal.Parse(p.AlbumPrice))
                .ToArray();

            var serAlbums = JsonConvert.SerializeObject(albums, new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });

            return serAlbums;
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs.Where(e => e.Duration.TotalSeconds > duration)
                .Select(s => new ExportSongDto
                {
                    SongName = s.Name,
                    Writer = s.Writer.Name,
                    Performer = s.SongPerformers.Select(p => p.Performer.FirstName + " " + p.Performer.LastName).FirstOrDefault(),
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(o => o.SongName)
                .ThenBy(f => f.Writer)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSongDto[]), new XmlRootAttribute("Songs"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });
            xmlSerializer.Serialize(new StringWriter(sb), songs, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}