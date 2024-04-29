import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LoginComponent } from '../login/login.component';
import { CommonModule } from '@angular/common';
import { SidebarModule } from 'primeng/sidebar';
import { PrimeIcons } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { HostListener } from "@angular/core";

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, LoginComponent, CommonModule, SidebarModule, ButtonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  isUserLoggedIn: boolean = false;
  isUserAdmin: boolean = false;
  sidebarVisible: boolean = false;
  windowWidth: number = window.innerWidth;
  navbarBreakpoint = +(getComputedStyle(document.body).getPropertyValue('--bs-breakpoint-lg')).slice(0, -2)

  constructor(private authenticationService: AuthenticationService) {  }

  ngOnInit(): void {
    this.authenticationService.isLoggedIn$.subscribe(
      res => this.isUserLoggedIn = res
    );
    this.authenticationService.isAdmin$.subscribe(
      res => this.isUserAdmin = res
    );
  }

  Logout() {
    this.authenticationService.setLoginStatus(false, '', false, false);
    this.isUserLoggedIn = false;
  }

  @HostListener('window:resize', ['$event'])
  getScreenWidth(event?: any) {
    this.windowWidth = window.innerWidth;
  }

  Search(searchString: HTMLInputElement) {

  }
}
