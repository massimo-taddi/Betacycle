import { Product } from "./Product";

export class ShoppingCartItem {
    public shoppingCartItemId: number;
    public shoppingCartId: number;
    public quantity: number;
    public productId: number;
    public createdDate: Date;
    public modifiedDate: Date;
    constructor() {
        this.shoppingCartItemId = 0;
        this.shoppingCartId = 0;
        this.quantity = 0;
        this.productId = 0;
        this.createdDate = new Date();
        this.modifiedDate = new Date();
    }
}
