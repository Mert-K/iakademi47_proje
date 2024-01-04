using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iakademi47_proje.Models
{
	public class Order
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

		[StringLength(30)]
		public string? OrderGroupGUID { get; set; }

		public int UserID { get; set; }
		public int ProductID { get; set; }
        public int Quantity { get; set; }


    }
}
