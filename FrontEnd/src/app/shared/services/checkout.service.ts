import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SalesOrderHeader } from '../models/SalesOrderHeader';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { KeyValuePair } from '../models/KeyValuePair';
import { ProductService } from './product.service';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  constructor(private http: HttpClient, private authService: AuthenticationService, private productService: ProductService) { }

  postSalesOrder(salesOrder: SalesOrderHeader): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post('https://localhost:7287/api/orders', salesOrder, {headers: header});
  }

  getOrderFreightCost(salesOrder: SalesOrderHeader, shipMethod: string): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    var weightQtyPairs: KeyValuePair<number | null, number>[] = [];
    
    salesOrder.salesOrderDetails.forEach((sod) => {
      weightQtyPairs.push(new KeyValuePair<number | null, number>(
        sod.productId, // peso del prodotto DA CORREGGERE
        sod.orderQty // qty del prodotto
      ));
    });
    return this.http.post(`https://localhost:7287/api/orders/freightcost/{shipMethod}`, weightQtyPairs, {headers: header});
  }
}
