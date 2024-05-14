import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer } from '../models/Customer';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthenticationService } from './authentication.service';
import { SignUpForm } from '../models/SignUpForm';
@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  constructor(private http: HttpClient, private auth: AuthenticationService) {}

  httpPostNewCustomer(customer: SignUpForm): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post('https://localhost:7287/api/customers', customer, {
      headers: header,
    });
  }
}
