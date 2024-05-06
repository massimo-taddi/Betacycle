import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../shared/models/Product';
import { ProductService } from '../../../shared/services/product.service';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SearchParams } from '../../../shared/models/SearchParams';

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [
    ButtonModule,
    RouterModule,
    CommonModule,
    FormsModule,
    PaginatorModule,
  ],
  templateUrl: './products-list.component.html',
  styleUrl: './products-list.component.css',
})
export class ProductsListComponent {
  products!: Product[];
  productsSuccessivi!: Product[];
  searchParams: SearchParams = new SearchParams();
  first: number = 0;
  rows: number = 10;

  constructor(private productService: ProductService) {}
  ngOnInit() {
    this.funzioneProdotti(1);
  }
  funzioneProdotti(index: number) {
    this.productService.searchParams$.subscribe(
      (par) => (this.searchParams = par)
    );
    this.searchParams.pageSize = 10;
    this.searchParams.pageIndex = index;
    this.productService.getProducts(this.searchParams).subscribe({
      next: (products: Product[]) => {
        this.products = products;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
  onPageChange(event: PaginatorState) {}
}
