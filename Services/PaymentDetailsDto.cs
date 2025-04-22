namespace wspay_v2.Services
 
{
    using System.ComponentModel.DataAnnotations;
    public class PaymentDetailsDto
    {
        public string ShopID { get; set; }
        public int ShoppingCartID { get; set; }
        [Required(ErrorMessage = "Version is required.")]
        public string Version { get; set; }
        public string TotalAmount { get; set; }
        public string Signature { get; set; }
        public string ReturnURL { get; set; }
        public string CancelURL { get; set; }
        public string ReturnErrorURL { get; set; }
    }
}