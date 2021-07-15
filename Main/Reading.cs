using System;
using System.ComponentModel.DataAnnotations;

namespace Main
{
    public class Reading
    {
        [StringLength(16), Required]
        public string PdmId { get; set; }
        [Required]
        public DateTime ReadingDateTime { get; set; }
        public string Comment { get; set; }
        public decimal rValue { get; set; }
        public DateTime taken { get; set; }
        public bool valid { get; set; }
        public DateTime ValidationDate { get; set; }
        public bool Zap { get; set; }
        public DateTime ZapDate { get; set; }
        [StringLength(16)]
        public string ZapAuthor { get; set; }

    }
}
