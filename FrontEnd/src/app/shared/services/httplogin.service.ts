import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class HttploginService {
  newHeader = new HttpHeaders({
    contentType: 'application/json',
    responseType: 'text',
  });
  constructor(private http: HttpClient) {}

  httpSendLoginCredentials(): Observable<any> {
    //modificare a post
    return this.http.get('');
  }
  httpValidateToken(): Observable<any> {
    //da finire
    return this.http.get('');
  }
}
