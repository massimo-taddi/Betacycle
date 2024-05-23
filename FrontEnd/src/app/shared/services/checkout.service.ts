import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SalesOrderHeader } from '../models/SalesOrderHeader';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  constructor(private http: HttpClient, private authService: AuthenticationService) { }

  postSalesOrder(salesOrder: SalesOrderHeader): Observable<any> {
    var header = new HttpHeaders();
    this.authService.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post('https://localhost:7287/api/orders', salesOrder, {headers: header});
  }
}
