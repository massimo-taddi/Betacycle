import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginCredentials } from '../models/LoginCredentials';

@Injectable({
  providedIn: 'root',
})
export class HttploginService {
  newHeader = new HttpHeaders({
    contentType: 'application/json',
    responseType: 'text',
  });
  constructor(private http: HttpClient) {}

  httpSendLoginCredentials(credential: LoginCredentials): Observable<any> {
    // when used: httpSendLoginCredentials(credentials).subscribe(
    //   (response: any) => {
    //     qualcosa = response.text();
    //   }
    // );
    return this.http.post('https://localhost:7287/app/login', credential);
  }

  httpValidateToken(credential: LoginCredentials): Observable<any> {
    //da finire
    //modificare a post
    this.newHeader = this.newHeader.set(
      'Authorization',
      'Basic '// + window.btoa(localStorage.getItem('credentials'))
    );
    return this.http.get('');
  }
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
