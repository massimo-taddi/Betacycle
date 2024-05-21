import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginCredentials } from '../models/LoginCredentials';
import { AuthenticationService } from './authentication.service';

@Injectable({
  providedIn: 'root',
})
export class HttploginService {
  constructor(private http: HttpClient, private auth: AuthenticationService) {}

  httpSendLoginCredentials(credentials: LoginCredentials): Observable<any> {
    return this.http.post('https://localhost:7287/app/loginjwt', credentials, { observe: "response", responseType: "text" });
  }

  httpLogoutTrace(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get('https://localhost:7287/app/LoginJwt/trace', {
      headers: header
    });
  }
}
