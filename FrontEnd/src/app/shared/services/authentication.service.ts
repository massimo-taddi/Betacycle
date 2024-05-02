import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private token = localStorage.getItem('jwtToken');
  private isLoggedIn = new BehaviorSubject(this.token != null);
  isLoggedIn$ = this.isLoggedIn.asObservable();
  private decodedToken: any = this.token != null ? jwtDecode(this.token!) : '';
  private isAdmin = new BehaviorSubject(this.token != null ? this.decodedToken.role == 'admin' : false);
  isAdmin$ = this.isAdmin.asObservable();
  
  // authJwtHeader = new HttpHeaders({
  //   contentType: 'application/json',
  //   responseType: 'text'
  // });
  private authJwtHeader = new BehaviorSubject(this.token != null ? new HttpHeaders({
    contentType: 'application/json',
    responseType: 'text',
    Authorization: 'Bearer ' + this.token
  }) : new HttpHeaders());
  authJwtHeader$ = this.authJwtHeader.asObservable();
  
  constructor() { }

  setLoginStatus(logValue: boolean, jwtToken: string='', stayLoggedIn: boolean = false, isAdmin: boolean = false) {
    this.isLoggedIn.next(logValue);
    this.isAdmin.next(isAdmin);
    if (logValue) {
      if(stayLoggedIn) {
        localStorage.setItem('jwtToken', jwtToken);
      }
      sessionStorage.setItem('jwtToken', jwtToken);
      // this.authJwtHeader = this.authJwtHeader.set(
      //   'Authorization',
      //   'Bearer ' + jwtToken
      // );
      this.authJwtHeader.next(new HttpHeaders({
        contentType: 'application/json',
        responseType: 'text',
        Authorization: 'Bearer ' + jwtToken
      }));
    } else {
      localStorage.removeItem('jwtToken');
      sessionStorage.removeItem('jwtToken');
      this.authJwtHeader.next(new HttpHeaders({
        contentType: 'application/json',
        responseType: 'text'
      }));
    }
  }

  getLoginStatus() {
    return this.isLoggedIn;
  }

  getAdminStatus() {
    return this.isAdmin;
  }
}
