import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginStatusService {
  private isLoggedIn = new BehaviorSubject(false);
  isLoggedIn$ = this.isLoggedIn.asObservable();
  private isAdmin = new BehaviorSubject(false);
  isAdmin$ = this.isAdmin.asObservable();

  setLoggedIn(isLoggedIn: boolean, stayLoggedIn?: boolean) {
    this.isLoggedIn.next(isLoggedIn);
    if(typeof(stayLoggedIn)  === undefined || stayLoggedIn === false){
      sessionStorage.setItem('isLoggedIn', isLoggedIn ? 'true' : 'false');
    }
    else {
      localStorage.setItem('isLoggedIn', isLoggedIn ? 'true' : 'false');
      sessionStorage.setItem('isLoggedIn', isLoggedIn ? 'true' : 'false');
    }
  }

  setAdmin(isAdmin: boolean, stayLoggedIn?: boolean) {
    this.isAdmin.next(isAdmin);
    if(typeof(stayLoggedIn)  === undefined || stayLoggedIn === false){
      sessionStorage.setItem('isAdmin', isAdmin ? 'true' : 'false');
    }
    else {
      localStorage.setItem('isAdmin', isAdmin ? 'true' : 'false');
      sessionStorage.setItem('isAdmin', isAdmin ? 'true' : 'false');
    }
  }
}
