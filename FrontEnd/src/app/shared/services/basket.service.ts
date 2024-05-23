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
  isLoggedIn: boolean = false;

  constructor(private authService: AuthenticationService, private http: HttpClient) { }

  getRemoteBasketItems(): Observable<any> { 
    this.authService.isLoggedIn$.subscribe((isLogged) => {
      this.isLoggedIn = isLogged;
    });
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    var items: ShoppingCartItem[] = [];
    if (this.isLoggedIn) {
      // get basket items from server
      return this.http.get('https://localhost:7287/api/shoppingcart', {headers: header});
    } else {
      // get basket items from local storage
      var localBasket = localStorage.getItem('basket');
      if(localBasket != undefined) {
        var localBasketFound = JSON.parse(localBasket) as ShoppingCartItem[];
        items = localBasketFound;
      }
      return of(items);
    }
   }

   postBasketItem(product: Product, quantity: number = 1): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    this.authService.isLoggedIn$.subscribe((isLogged) => {
      this.isLoggedIn = isLogged;
    });
    let item: ShoppingCartItem = new ShoppingCartItem();
    item.productId = product.productId;
    item.createdDate = new Date(Date.now());
    item.modifiedDate = new Date(Date.now());
    item.quantity = quantity;
    if(this.isLoggedIn) {
      return this.postBasketItemRemote(item);
    } else {
      return this.postBasketItemLocal(item);
    }
   }

   postBasketItemLocal(product: ShoppingCartItem): Observable<any> {
    // logica per aggiungere + di 1 prodotto facendo aumentare la quantita'
    var localBasket = localStorage.getItem('basket');
    // il basket esiste su localstorage
    if(localBasket != undefined) {
      var localBasketFound = JSON.parse(localBasket) as ShoppingCartItem[];
      // se il prodotto e' gia' presente nel basket
      if(localBasketFound?.find((p: ShoppingCartItem) => p.productId == product.productId) != undefined) {
        localBasketFound?.map((p: ShoppingCartItem) => {
          if(p.productId == product.productId) {
            p.quantity += product.quantity;
          }
        });
      } else {
        localBasketFound.push(product);
      }
      localStorage.setItem('basket', JSON.stringify(localBasketFound));
      // basket non esiste su localstorage
    } else {
      var newBasket: ShoppingCartItem[] = [];
      newBasket.push(product);
      localStorage.setItem('basket', JSON.stringify(newBasket));
    }
    return of(product);
   }

   postBasketItemRemote(product: ShoppingCartItem, fromLocalBasket: boolean = false): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post(`https://localhost:7287/api/shoppingcart${fromLocalBasket ? '?postFromLocal=true' : ''}`, product, {headers: header});
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

   userHasBasket(): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get('https://localhost:7287/api/shoppingcart/hascart', {headers: header});
   }

   deleteBasketItem(itemId: number): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.delete(`https://localhost:7287/api/shoppingcart/${itemId}`, {headers: header});
   }

   pushLocalCart() {
    var localBasket = localStorage.getItem('basket');
    if(localBasket != undefined) { //user non loggato ha il basket in local
      var localBasketFound = JSON.parse(localBasket) as ShoppingCartItem[];
      this.userHasBasket().subscribe((response: boolean) => {
        if(!response) { //basket in local presente ma non sul db
          this.postBasketItemRemote(localBasketFound[0]!, true).subscribe({
            next: (resp: any) =>{
              if(resp != null && localBasketFound.length > 1){
                for(var i=1; i< localBasketFound.length; i++){
                  this.postBasketItemRemote(localBasketFound[i], true).subscribe();
                }
              }
            },
            error: (err: Error) =>{
              console.log(err)
            }
          });
        }
        localStorage.removeItem('basket');
      });
    }
  }
}
