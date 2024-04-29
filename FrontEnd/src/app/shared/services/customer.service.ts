import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer } from '../models/Customer';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private http: HttpClient) { }

  httpPostNewCustomer(customer: Customer): Observable<any> {
    return this.http.post('https://localhost:7287/api/customer', customer);
  }
}
