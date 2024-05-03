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
  constructor(private productService: ProductService) {}
}
