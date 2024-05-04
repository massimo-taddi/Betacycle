import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../shared/models/Product';
import { ProductService } from '../../../shared/services/product.service';
import { PaginatorModule } from 'primeng/paginator';
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
  searchParams: SearchParams = new SearchParams();
  first: number = 0;
  rows: number = 10;
  productCount: number = 0;

  constructor(private productService: ProductService) {}
  ngOnInit() {
    this.funzioneProdotti(1);
  }
  funzioneProdotti(index: number) {
    this.searchParams.pageIndex = 1;
    this.searchParams.pageSize = 10;
    this.productService.searchParams$.subscribe(
      (par) => (this.searchParams = par)
    );
    this.productService.getProducts(this.searchParams).subscribe({
      next: (products: any) => {
        this.products = products.item2;
        this.productCount = products.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }

  changeOutput(event: any) {
    this.searchParams.pageIndex = event.page! + 1;
    this.searchParams.pageSize = event.rows!;
    this.productService.searchParams$.subscribe(
      (par) => (this.searchParams = par)
    );
    this.productService.getProducts(this.searchParams).subscribe({
      next: (products: any) => {
        this.products = products.item2;
        this.productCount = products.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
}
