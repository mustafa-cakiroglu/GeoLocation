using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GeoLocationTest.Models
{
    [Table("Cm_CustomerLocations", Schema = "dbo")]
    public class Cm_CustomerLocations
    {
        public Cm_CustomerLocations()
        {
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CustomerId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Cm_Customer Customer { get; set; }

    }
}
