using _1.WorkingMVC.Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1.WorkingMVC.Data.Entities
{
	[Table("tblCarts")]
	public class CartEntity
	{
		[ForeignKey(nameof(Product))]
		public int ProductId { get; set; }
		[ForeignKey(nameof(User))]
		public int UserId { get; set; }
		public int Quantity { get; set; }
		public virtual ProductEntity? Product { get; set; }
		public virtual UserEntity? User { get; set; }
	}
}
