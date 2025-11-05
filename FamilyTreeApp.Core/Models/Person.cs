using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTreeApp.Core.Models
{
    // Representa un miembro de la familia con toda su información personal
    public class Person
    {
        public string Id { get; set; } // para identificar
        public string FullName { get; set; }
        public string IdNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public bool IsAlive { get; set; }
        public string PhotoPath { get; set; }
        public GeoCoordinates Residence { get; set; }



        public string FatherId { get; set; }
        public string MotherId { get; set; }
        public List<string> ChildrenIds { get; set; }

        public Person()
        {
            Id = Guid.NewGuid().ToString();
            ChildrenIds = new List<string>();
            IsAlive = true;
            Residence = new GeoCoordinates();
        }
        // Calcula edad actual
        public void CalculateAge()
        {
            var today = DateTime.Today;
            Age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-Age)) Age--;
        }
    }


    public class GeoCoordinates // por ahora es literalmente escribir la ubicación :v puntos extra guiño guiño
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }

        public GeoCoordinates()
        {
            Address = string.Empty;
        }
        public GeoCoordinates(double latitude, double longitude, string address = "")
        {
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
        }
    }
}