import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PrimeIcons, MenuItem } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { SidebarModule } from 'primeng/sidebar';
import { HostListener } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-private-area',
  standalone: true,
  imports: [RouterModule,CommonModule, SidebarModule, ButtonModule],
  templateUrl: './private-area.component.html',
  styleUrl: './private-area.component.css'
})
export class PrivateAreaComponent {
  windowWidth: number = window.innerWidth;
  navbarBreakpoint =+(getComputedStyle(document.body).getPropertyValue('--bs-breakpoint-xl')).slice(0, -2);
  mySidebar: boolean = false;

  @HostListener('window:resize', ['$event'])
  getScreenWidth(event?: any) {
    this.windowWidth = window.innerWidth;
  }


  StatusPage(currentPage: string){
    switch(currentPage){
      case 'info':
        document.getElementById('info')?.setAttribute('class', 'nav-link text-white active');
        document.getElementById('orders')?.setAttribute('class', 'nav-link text-white');
        document.getElementById('addresses')?.setAttribute('class', 'nav-link text-white');
        break;
      case 'orders':
        document.getElementById('orders')?.setAttribute('class', 'nav-link text-white active');
        document.getElementById('info')?.setAttribute('class', 'nav-link text-white');
        document.getElementById('addresses')?.setAttribute('class', 'nav-link text-white');
        break;
      case 'addresses':
        document.getElementById('addresses')?.setAttribute('class', 'nav-link text-white active');
        document.getElementById('orders')?.setAttribute('class', 'nav-link text-white');
        document.getElementById('info')?.setAttribute('class', 'nav-link text-white');
        break;
    }
  }

  getJwtUsername():string {
    if(sessionStorage.getItem('jwtToken') == null) return 'user'
    var decodedToken: any = jwtDecode(sessionStorage.getItem('jwtToken')!);
    return decodedToken.unique_name;
  }
}
