import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../shared/models/Product';
import { ProductService } from '../../../shared/services/product.service';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SearchParams } from '../../../shared/models/SearchParams';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [
    ButtonModule,
    RouterModule,
    CommonModule,
    FormsModule,
    PaginatorModule,
    CardModule,
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
  searchProduct: string = '';
  constructor(private productService: ProductService) {}
  ngOnInit() {
    this.funzioneProdotti();
  }
  funzioneProdotti() {
    this.searchParams.search = this.searchProduct;
    this.searchParams.sort = 'Desc';

    this.productService.getProductsAdmin(this.searchParams).subscribe({
      next: (prod: any) => {
        this.products = prod.item2;
        this.productCount = prod.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }

  changeOutput(event: any) {
    this.searchParams.pageIndex = event.page! + 1;
    this.searchParams.pageSize = event.rows!;

    window.scroll({
      top: 0,
      left: 0,
      behavior: 'auto',
    });

    this.productService.getProductsAdmin(this.searchParams).subscribe({
      next: (products: any) => {
        this.products = products.item2;
        this.productCount = products.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
  onPageChange(event: PaginatorState) {}

  Delete() {
    this.productService;
  }
  SetLocalStorage(modifyId: number) {
    let modifyIdString = modifyId.toString();
    sessionStorage.setItem('ModifyId', modifyIdString);
  }
}
