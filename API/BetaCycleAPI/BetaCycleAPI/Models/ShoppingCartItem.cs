using System;
using System.Collections.Generic;

namespace BetaCycleAPI.Models;

public partial class ShoppingCartItem
{
    public int ShoppingCartItemId { get; set; }

    public int ShoppingCartId { get; set; }

    public int Quantity { get; set; }

    public int ProductId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public Guid Rowguid { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
}
