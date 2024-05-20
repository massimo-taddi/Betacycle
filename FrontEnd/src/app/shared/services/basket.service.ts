import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { ShoppingCartItem } from '../models/ShoppingCartItem';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Product } from '../models/Product';

@Injectable({
  providedIn: 'root'
})
export class BasketService {

  constructor(private authService: AuthenticationService, private http: HttpClient) { }

  getRemoteBasketItems(): Observable<any> { 
    var isLoggedIn;
    this.authService.isLoggedIn$.subscribe((isLogged) => {
      isLoggedIn = isLogged;
    });
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    var items: ShoppingCartItem[] = [];
    if (isLoggedIn) {
      // get basket items from server
      return this.http.get('https://localhost:7287/api/shoppingcart', {headers: header});
    } else {
      // get basket items from local storage
      // ancora da fare
      return of(items);
    }
   }

   postBasketItem(product: Product): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    let item: ShoppingCartItem = new ShoppingCartItem();
    item.productId = product.productId;
    item.createdDate = new Date(Date.now());
    item.modifiedDate = new Date(Date.now());
    item.quantity = 1;
    return this.http.post('https://localhost:7287/api/shoppingcart', item, {headers: header});
   }

   putBasketItem(item: ShoppingCartItem): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.put(`https://localhost:7287/api/shoppingcart/${item.shoppingCartItemId}`, item, {headers: header});
   }

   isItemInBasket(productId: number): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get(`https://localhost:7287/api/shoppingcart/isproductadded?productId=${productId}`, {headers: header});
   }

   deleteBasketItem(itemId: number): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.delete(`https://localhost:7287/api/shoppingcart/${itemId}`, {headers: header});
   }
}
