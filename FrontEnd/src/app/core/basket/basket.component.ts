import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ShoppingCartItem } from '../../shared/models/ShoppingCartItem';
import { BasketService } from '../../shared/services/basket.service';
import { ProductService } from '../../shared/services/product.service';
import { Product } from '../../shared/models/Product';

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './basket.component.html',
  styleUrl: './basket.component.css'
})
export class BasketComponent implements OnInit {
  basketItems: ShoppingCartItem[] = [];

  constructor(private shoppingCartService: BasketService, private productService: ProductService) { }

  ngOnInit() {
    this.basketItems = this.shoppingCartService.getBasketItems();
  }

  calculateTotalPrice() { }
  getProductName(productId: number): string { 
    var prodName: string = '';
    this.productService.getProductById(productId).subscribe({
      next: (prod: Product) => {
        prodName = prod.name;
      },
      error: (err: Error) => {
        console.log(err.message);
      }
    });
    return prodName;
  }
  getProductPrice(productId: number): number { 
    var prodPrice: number = 0;
    this.productService.getProductById(productId).subscribe({
      next: (prod: Product) => {
        prodPrice = prod.listPrice;
      },
      error: (err: Error) => {
        console.log(err.message);
      }
    });
    return prodPrice;
  }
}
