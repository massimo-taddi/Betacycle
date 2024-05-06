import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer } from '../models/Customer';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  constructor(private http: HttpClient, private auth: AuthenticationService) {}

  httpPostNewCustomer(customer: Customer): Observable<any> {
    return this.http.post('https://localhost:7287/api/customer', customer);
  }
}
