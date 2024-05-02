import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root'
})
export class HttpUserAdminService {

  constructor(private http: HttpClient, private auth: AuthenticationService) { }

  // if user is admin, gets all orders in the db. if user is not admin, gets their personal orders
  httpGetUserOrders(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe(
      h => header = h
    );
    return this.http.get('https://localhost:7287/api/orders', {headers: header});
  }

  // if user is admin, get every customer's information. if user is customer, get a list with only their information in it
  httpGetCustomerInfo(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe(
      h => header = h
    );
    return this.http.get('https://localhost:7287/api/customers', {headers: header});
  }

  httpGetCustomerAddresses(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe(
      h => header = h
    );
    return this.http.get('https://localhost:7287/api/addresses', {headers: header});
  }
}
