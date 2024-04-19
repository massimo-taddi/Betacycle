import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LoginComponent } from '../login/login.component';
import { CommonModule } from '@angular/common';
import { LoginStatusService } from '../../shared/services/login-status.service';
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, LoginComponent, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
}
