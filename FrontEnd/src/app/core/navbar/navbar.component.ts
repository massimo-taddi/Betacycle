import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LoginComponent } from '../login/login.component';
import { CommonModule } from '@angular/common';
import { LoginStatusService } from '../../shared/services/login-status.service';
import { SidebarModule } from 'primeng/sidebar';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, LoginComponent, CommonModule, SidebarModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  isUserLoggedIn: boolean = false;
  isUserAdmin: boolean = false;
  sidebarVisible: boolean = false;

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
  Search(searchString: HTMLInputElement) {

  }
}
