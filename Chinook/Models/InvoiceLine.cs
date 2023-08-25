using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Models
{
    public partial class InvoiceLine
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InvoiceLineId { get; set; }
        public long InvoiceId { get; set; }
        public long TrackId { get; set; }
        public byte[] UnitPrice { get; set; } = null!;
        public long Quantity { get; set; }

        public virtual Invoice Invoice { get; set; } = null!;
        public virtual Track Track { get; set; } = null!;
    }
}
