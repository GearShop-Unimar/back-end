using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GearShop.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int CartId { get; set; }

        // AQUI: Adicionei o ?
        public Cart? Cart { get; set; }

        public int ProductId { get; set; }

        // AQUI: Adicionei o ?
        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}
