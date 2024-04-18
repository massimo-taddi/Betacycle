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

  setLoggedIn(isLoggedIn: boolean) {
    this.isLoggedIn.next(isLoggedIn);
    sessionStorage.setItem('isLoggedIn', isLoggedIn ? 'true' : 'false');
  }

  setAdmin(isAdmin: boolean) {
    this.isAdmin.next(isAdmin);
    sessionStorage.setItem('isAdmin', isAdmin ? 'true' : 'false');
  }
}
