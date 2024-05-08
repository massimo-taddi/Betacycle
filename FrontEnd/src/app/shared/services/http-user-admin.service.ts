import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { PwResetCreds } from '../models/PwResetCreds';
import { SearchParams } from '../models/SearchParams';
import { Address } from '../models/Address';
import { AddressPost } from '../models/AddressPost';

@Injectable({
  providedIn: 'root',
})
export class HttpUserAdminService {
  private searchParams = new BehaviorSubject(new SearchParams());
  searchParams$ = this.searchParams.asObservable();
  constructor(private http: HttpClient, private auth: AuthenticationService) {}

  // if user is admin, gets all orders in the db. if user is not admin, gets their personal orders
  httpGetUserOrders(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get('https://localhost:7287/api/orders', {
      headers: header,
    });
  }

  // if user is admin, get every customer's information. if user is customer, get a list with only their information in it
  httpGetCustomerInfo(
    params: SearchParams = new SearchParams()
  ): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get(
      `https://localhost:7287/api/Customers?PageIndex=${params.pageIndex}&PageSize=${params.pageSize}&Sort=${params.sort}&Search=${params.search}`,
      {
        headers: header,
      }
    );
  }

  httpGetCustomerAddresses(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get('https://localhost:7287/api/addresses', {
      headers: header,
    });
  }
  setSearchParams(params: SearchParams) {
    this.searchParams.next(params);
  }

  httpPostCustomerAddress(newAddress: AddressPost): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post(
      'https://localhost:7287/api/addresses',
      newAddress,
      {
        headers: header,
      }
    );
  }

  httpUserResetPassword(oldAndNew: PwResetCreds): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.put(
      'https://localhost:7287/api/passwordreset/loggedin',
      oldAndNew,
      {
        headers: header,
      }
    );
  }

  httpSendResetEmail(email: string): Observable<any> {
    return this.http.post(
      'https://localhost:7287/api/passwordreset/forgot?email='+ email,
      null
    );
  }

  httpTempUserResetPassword(newPwd: string, tempToken: string): Observable<any> {
    return this.http.put(
      'https://localhost:7287/api/passwordreset/notloggedin?newpwd=' + newPwd,
      null, { 
        headers: new HttpHeaders({
          contentType: 'application/json',
          responseType: 'text',
          Authorization: 'Bearer ' + tempToken
        })
      }
    );
  }
}
