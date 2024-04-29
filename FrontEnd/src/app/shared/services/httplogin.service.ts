import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginCredentials } from '../models/LoginCredentials';
import { Customer } from '../models/Customer';

@Injectable({
  providedIn: 'root',
})
export class HttploginService {
  newHeader = new HttpHeaders({
    contentType: 'application/json',
    responseType: 'text',
  });
  constructor(private http: HttpClient) {}

  httpSendLoginCredentials(credentials: LoginCredentials): Observable<any> {
    return this.http.post('https://localhost:7287/app/loginjwt', credentials, { observe: "response", responseType: "text" });
  }

  // httpValidateToken(): Observable<any> {
  //   //da finire
  //   //modificare a post
  //   this.newHeader = this.newHeader.set(
  //     'Authorization',
  //     'Basic '// + window.btoa(localStorage.getItem('credentials'))
  //   );
  //   return this.http.get('');
  // }
  /*
  httpGetAuthrsById(id: number): Observable<any> {
    this.newHeader = this.newHeader.set(
      'Authorization',
      'Basic' + window.btoa('Claudio:orloff')
    );
    localStorage.setItem;
    sessionStorage.getItem;
    return this.http.get(`https://localhost:7132/api/Authors/${id}`, {
      headers: this.newHeader,
    });
  }
  */
}
