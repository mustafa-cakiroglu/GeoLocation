using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GeoLocationTest.Models
{
    [Table("Cm_Customer", Schema = "dbo")]
    public class Cm_Customer
    {
        public Cm_Customer()
        {
            CmCustomerLocations = new HashSet<Cm_CustomerLocations>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ICollection<Cm_CustomerLocations> CmCustomerLocations { get; set; }

    }
}
