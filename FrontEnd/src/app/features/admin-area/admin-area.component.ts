import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { InputGroupModule } from 'primeng/inputgroup';
import { FormsModule } from '@angular/forms';
import { Product } from '../../shared/models/Product';
import { ProductService } from '../../shared/services/product.service';
import { SearchParams } from '../../shared/models/SearchParams';
import { AdminFunctionalitiesComponent } from './admin-functionalities/admin-functionalities.component';
import { AuthenticationService } from '../../shared/services/authentication.service';
@Component({
  selector: 'app-admin-area',
  standalone: true,
  imports: [
    ButtonModule,
    RouterModule,
    CommonModule,
    InputGroupModule,
    FormsModule,
    AdminFunctionalitiesComponent,
  ],
  templateUrl: './admin-area.component.html',
  styleUrl: './admin-area.component.css',
})
export class AdminAreaComponent {
  products!: Product[];
  searchParams: SearchParams = new SearchParams();
  isUserAdmin: boolean = false;
  isUserLoggedIn: boolean = false;
  constructor(
    private productService: ProductService,
    private authenticationService: AuthenticationService
  ) {}
  ngOnInit(): void {
    this.authenticationService.isLoggedIn$.subscribe(
      (res) => (this.isUserLoggedIn = res)
    );
    if (localStorage.getItem('jwtToken') != null)
      sessionStorage.setItem('jwtToken', localStorage.getItem('jwtToken')!);
    this.authenticationService.isAdmin$.subscribe(
      (res) => (this.isUserAdmin = res)
    );
  }
}
