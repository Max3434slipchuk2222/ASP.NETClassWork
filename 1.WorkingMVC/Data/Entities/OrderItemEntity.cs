using System.ComponentModel.DataAnnotations.Schema;

namespace _1.WorkingMVC.Data.Entities;
[Table("tblOrderItems")]
public class OrderItemEntity : BaseEntity<int>
{
	public decimal PriceBuy { get; set; }
	public int Count { get; set; }
	[ForeignKey(nameof(Product))]
	public int ProductId { get; set; }
	[ForeignKey(nameof(Order))]
	public int OrderId { get; set; }
	public ProductEntity? Product { get; set; }
	public OrderEntity? Order { get; set; }


}
