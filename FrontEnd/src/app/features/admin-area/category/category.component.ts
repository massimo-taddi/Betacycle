import { Component, OnInit } from '@angular/core';
import { PaginatorModule } from 'primeng/paginator';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SearchParams } from '../../../shared/models/SearchParams';
import { ProductCategory } from '../../../shared/models/ProductCategory';

import { ProductService } from '../../../shared/services/product.service';
@Component({
  selector: 'app-category',
  standalone: true,
  imports: [
    PaginatorModule,
    CardModule,
    FormsModule,
    ButtonModule,
    CommonModule,
  ],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css',
})
export class CategoryComponent {
  categories!: ProductCategory[];
  searchParams: SearchParams = new SearchParams();
  first: number = 0;
  rows: number = 10;
  categoriesCount: number = 0;

  constructor(private service: ProductService) {}
  ngOnInit() {
    this.service.getProductCategories().subscribe({
      next: (category: any) => {
        this.categories = category;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }

  //modificare la chiamata a controller
  changeOutput(event: any) {
    this.service.searchParams$.subscribe((par) => (this.searchParams = par));

    this.searchParams.pageIndex = event.page! + 1;
    this.searchParams.pageSize = event.rows!;

    window.scroll({
      top: 0,
      left: 0,
      behavior: 'auto',
    });

    this.service.getProducts(this.searchParams).subscribe({
      next: (category: any) => {
        this.categories = category.item2;
        this.categoriesCount = category.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
}
