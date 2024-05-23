import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SalesOrderHeader } from '../models/SalesOrderHeader';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  constructor(private http: HttpClient) { }

  postSalesOrder(salesOrder: SalesOrderHeader): Observable<any> {
    return this.http.post('https://localhost:7287/api/orders', salesOrder);
  }
}
