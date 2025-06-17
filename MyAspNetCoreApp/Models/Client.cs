using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAspNetCoreApp.Models
{
    public class Client
    {
        public Client()
        {
            Id = string.Empty;
            ClientName = string.Empty;
            MainBusiness = string.Empty;
            Address = string.Empty;
            Website = string.Empty;
            ContactPerson = string.Empty;
        }

        [Key]
        [Column("ClientID")]
        public string Id { get; set; }

        [Required]
        [Column("ClientName")]
        [StringLength(100)]
        public string ClientName { get; set; }

        [Column("MainBusiness")]
        [StringLength(200)]
        public string MainBusiness { get; set; }

        [Column("Address")]
        [StringLength(200)]
        public string Address { get; set; }

        [Column("Website")]
        [StringLength(100)]
        public string Website { get; set; }

        [Column("ContactPerson")]
        [StringLength(100)]
        public string ContactPerson { get; set; }

        [Column("ExpDate")]
        public DateTime ExpDate { get; set; }

        [Column("Active")]
        public bool Active { get; set; }
    }
}
