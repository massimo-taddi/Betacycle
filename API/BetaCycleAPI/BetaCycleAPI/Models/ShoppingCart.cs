using System;
using System.Collections.Generic;

namespace BetaCycleAPI.Models;

public partial class ShoppingCart
{
    public int ShoppingCartId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public Guid Rowguid { get; set; }

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
