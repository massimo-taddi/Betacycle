import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ShoppingCartItem } from '../../shared/models/ShoppingCartItem';
import { BasketService } from '../../shared/services/basket.service';
import { ProductService } from '../../shared/services/product.service';
import { Product } from '../../shared/models/Product';
import { FormsModule, NgModel } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [CommonModule, DropdownModule, FormsModule, ToastModule],
  templateUrl: './basket.component.html',
  styleUrl: './basket.component.css',
  providers: [MessageService]
})
export class BasketComponent implements OnInit {
  basketItemsProductsMap: Map<ShoppingCartItem, Product> = new Map();
  rangeArray = [...Array(10)].map((_, i) => 1 + i * 1);

  constructor(private shoppingCartService: BasketService, private productService: ProductService, private messageService: MessageService) { }

  ngOnInit() {
    this.fillBasket();
  }

  updateQuantity(item: ShoppingCartItem) {
    this.shoppingCartService.putBasketItem(item).subscribe({
      error: (err: Error) => {
        console.log(err.message);
      }
    });
  }

  private fillBasket() {
    this.shoppingCartService.getRemoteBasketItems().subscribe({
      next: (items: ShoppingCartItem[]) => {
        var basketItems = items;
        basketItems.forEach((item) => {
          this.productService.getProductById(item.productId).subscribe({
            next: (product: Product) => {
              this.basketItemsProductsMap.set(item, product);
            },
            error: (err: Error) => {
              console.log(err.message);
            }
          });
        });
      },
      error: (err: Error) => {
        console.log(err.message);
      }
    });
  }

  deleteItem(item: ShoppingCartItem) {
    this.shoppingCartService.deleteBasketItem(item.productId).subscribe({
      next: () => {
        this.basketItemsProductsMap.delete(item);
        this.showItemDeletedMessage();
      },
      error: (err: Error) => {
        console.log(err.message);
      }
    });
  }

  showItemDeletedMessage() {
    this.messageService.add({ severity: 'success', detail: 'Item deleted' });
  }

  calculateTotalPrice(): number { 
    var totalPrice = 0;
    this.basketItemsProductsMap.forEach((product, item) => {
      totalPrice += (product.listPrice * item.quantity);
    });
    return totalPrice;
  }
}
