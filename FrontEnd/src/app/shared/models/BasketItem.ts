import { Product } from "./Product";

export class BasketItem {
    productName: string = '';
    price: number = 0;
    quantity: number = 0;
    total: number = this.price * this.quantity;
}