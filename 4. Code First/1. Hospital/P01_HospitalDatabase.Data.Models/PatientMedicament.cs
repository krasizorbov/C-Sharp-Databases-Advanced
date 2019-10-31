using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
    public class PatientMedicament
    {
        [Key, Column(Order = 0)]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Key, Column(Order = 1)]
        public int MedicamentId { get; set; }
        public Medicament Medicament { get; set; }

        
    }
}
