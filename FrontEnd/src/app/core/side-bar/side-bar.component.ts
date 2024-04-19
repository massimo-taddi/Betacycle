import { Component } from '@angular/core';
import { LoginStatusService } from '../../shared/services/login-status.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-side-bar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.css'
})
export class SideBarComponent {
  isUserLoggedIn: boolean = false;
  isUserAdmin: boolean = false;

  constructor(private loginStatus: LoginStatusService) { }

  ngOnInit(): void {
    if(sessionStorage.getItem('isLoggedIn') === 'true' || localStorage.getItem('isLoggedIn') === 'true') {
      this.isUserLoggedIn = true;
      if(sessionStorage.getItem('isAdmin') === 'true' || localStorage.getItem('isAdmin') === 'true')
        this.isUserAdmin = true;
      return;
    }
    this.loginStatus.isLoggedIn$.subscribe(
      res => this.isUserLoggedIn = res
    );
    this.loginStatus.isAdmin$.subscribe(
      res => this.isUserAdmin = res
    );
  }

  Logout() {
    this.loginStatus.setAdmin(false);
    this.loginStatus.setLoggedIn(false);
    localStorage.removeItem('credentials');
    sessionStorage.removeItem('credentials');
    this.isUserLoggedIn = false;
    //window.location.reload();
  }
}
