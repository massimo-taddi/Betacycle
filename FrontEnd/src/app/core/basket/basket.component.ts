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
import { AuthenticationService } from '../../shared/services/authentication.service';
import { DialogModule } from 'primeng/dialog';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [CommonModule, DropdownModule, FormsModule, ToastModule, DialogModule, RouterModule],
  templateUrl: './basket.component.html',
  styleUrl: './basket.component.css',
  providers: [MessageService]
})
export class BasketComponent implements OnInit {
  basketItemsProductsMap: Map<ShoppingCartItem, Product> = new Map();
  rangeArray = [...Array(10)].map((_, i) => 1 + i * 1);
  isUserLoggedIn: boolean = false;
  templateCheckout: boolean = false;
  noItemsWarning: boolean = false;


  constructor(private shoppingCartService: BasketService, private productService: ProductService, private messageService: MessageService,
              private authenticationService: AuthenticationService, private router: Router) { }

  ngOnInit() {
    this.fillBasket();
    this.authenticationService.isLoggedIn$.subscribe(
      (res) => (this.isUserLoggedIn = res)
    );
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


  CheckoutRoute(){
    if(this.basketItemsProductsMap.size != 0){
      if(!this.isUserLoggedIn){
        this.noItemsWarning = false;
        this.templateCheckout = true;
      }else{
        this.router.navigate(["/checkout"]);
      }
    }else{
      this.noItemsWarning = true;
    }
  }
}
