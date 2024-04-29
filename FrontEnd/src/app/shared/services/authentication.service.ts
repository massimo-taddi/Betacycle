import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private isLoggedIn = new BehaviorSubject(false);
  isLoggedIn$ = this.isLoggedIn.asObservable();
  private isAdmin = new BehaviorSubject(false);
  isAdmin$ = this.isAdmin.asObservable();
  
  // authJwtHeader = new HttpHeaders({
  //   contentType: 'application/json',
  //   responseType: 'text'
  // });
  private authJwtHeader = new BehaviorSubject(new HttpHeaders());
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
      // this.authJwtHeader= new BehaviorSubject(new HttpHeaders({
      //   contentType: 'application/json',
      //   responseType: 'text'
      // }));
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
