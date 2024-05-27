import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SalesOrderHeader } from '../models/SalesOrderHeader';
import { BehaviorSubject, Observable, lastValueFrom } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { KeyValuePair } from '../models/KeyValuePair';
import { ProductService } from './product.service';
import { SalesOrderDetail } from '../models/SalesOrderDetail';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  postResultOrderHeader$: Observable<any> = new Observable();

  constructor(private http: HttpClient, private authService: AuthenticationService, private productService: ProductService) { }

  postSalesOrder(salesOrder: SalesOrderHeader): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    this.postResultOrderHeader$ = this.http.post('https://localhost:7287/api/orders', salesOrder, {headers: header});
    return this.postResultOrderHeader$;
  }

  private weightsFromDetails(salesOrderDetails: SalesOrderDetail[]): Observable<any> {
    var weightQtyPairs: KeyValuePair<number | null, number>[] = [];
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post(`https://localhost:7287/api/orders/weightsfromdetails`, salesOrderDetails, {headers: header});
  }

  async getOrderFreightCost(salesOrder: SalesOrderHeader, shipMethod: string): Promise<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    var weightQtyPairs: KeyValuePair<number | null, number>[] = [];
    
    // usare la chiamata api per ottenere un array di KeyValuePair con il peso e la quantità di ogni prodotto.
    // in questo modo si evita di fare una chiamata per ogni prodotto
    var weights$ = this.weightsFromDetails(salesOrder.salesOrderDetails) //.subscribe({
    //   next: (data: KeyValuePair<number | null, number>[]) => {
    //     weightQtyPairs = data;
    //   },
    //   error: (err: Error) => {
    //     console.log(err.message);
    //   }
    // });
    weightQtyPairs = await lastValueFrom(weights$);
    console.log(weightQtyPairs);
    return lastValueFrom(this.http.post(`https://localhost:7287/api/orders/freightcost/${shipMethod}`, weightQtyPairs, {headers: header}));
  }
}
