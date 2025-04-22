using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace wspay_v2.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int PaymentId { get; set; }
        public string ShopID { get; set; } = "TESTBIGC";
        public int ShoppingCartID { get; set; }
        [StringLength(10, ErrorMessage = "Version cannot exceed 10 characters.")]
        public string Version { get; set; } = "2.0";
        public string TotalAmount { get; set; }
        public string Signature { get; set; }
        public string ReturnURL { get; set; }
        public string CancelURL { get; set; }
        [Url(ErrorMessage = "Return Error URL must be a valid URL.")]
        public string ReturnErrorURL { get; set; }
        private Payment()
        {

        }

        public static Payment Create(string shopID, int shoppingCartID, string version, string totalAmount, string returnURL, string cancelURL, string returnErrorURL)
        {
            if (string.IsNullOrEmpty(shopID))
                throw new ArgumentNullException(nameof(shopID));

            var payment = new Payment
            {
                ShopID = shopID,
                ShoppingCartID = shoppingCartID,
                Version = version,
                TotalAmount = totalAmount,
                ReturnURL = returnURL,
                CancelURL = cancelURL,
                ReturnErrorURL = returnErrorURL
            };

            payment.Signature = payment.GenerateSignature();
            return payment;
        }

        public string GenerateSignature()
        {
            string secretKey = "074c6ca334fb4W";
            string rawData =ShopID + secretKey + ShoppingCartID + secretKey + TotalAmount.ToString().Replace(",", "") + secretKey;
            return sha512(rawData);
        }

        private string sha512(string input)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
