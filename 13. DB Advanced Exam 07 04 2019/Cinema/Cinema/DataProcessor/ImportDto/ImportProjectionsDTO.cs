﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Projection")]
    public class ImportProjectionsDTO
    {
        [XmlElement("MovieId")]
        public int MovieId { get; set; }
        [XmlElement("HallId")]
        public int HallId { get; set; }
        [XmlElement("DateTime")]
        [Required]
        public string DateTime { get; set; }
    }
}
