using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace wspay_v2.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int PaymentId { get; set; }
        public string ShopID { get; set; } = "TESTBIGC";
        public int ShoppingCartID { get; set; }
        public string Version { get; set; } = "2.0";
        public string TotalAmount { get; set; }
        public string Signature { get; set; }
        public string ReturnURL { get; set; }
        public string CancelURL { get; set; }
        public string ReturnErrorURL { get; set; }
        public Payment()
        {

        }
    }
}
