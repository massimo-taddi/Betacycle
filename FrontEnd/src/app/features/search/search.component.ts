import { Component } from '@angular/core';
import { SidebarModule } from 'primeng/sidebar';
import { FormsModule } from '@angular/forms';
import { RatingModule } from 'primeng/rating';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { Product } from '../../shared/models/Product';
import { ProductService } from '../../shared/services/product.service';
import { CommonModule } from '@angular/common';
import { SearchParams } from '../../shared/models/SearchParams';
import { PaginatorModule } from 'primeng/paginator';
@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    SidebarModule,
    FormsModule,
    DataViewModule,
    RatingModule,
    TagModule,
    CommonModule,
    PaginatorModule
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css',
})
export class SearchComponent {
  products!: Product[];
  searchParams: SearchParams = new SearchParams();
  productCount: number =0;

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.searchParams.pageIndex = 1;
    this.productService.searchParams$.subscribe(
      par => this.searchParams = par
    );
    this.productService
      .getProducts(this.searchParams).subscribe({
        next: (products: any) => {
          this.products = products.item2;
          this.productCount = products.item1;
        },
        error: (err: Error) => {
          console.log(err.message);
        },
      });
  }

  changeOutput(event: any){
    this.searchParams.pageIndex = event.page+1;
    this.searchParams.pageSize = event.rows;
    this.productService.searchParams$.subscribe(
      par => this.searchParams = par
    );
    this.productService
      .getProducts(this.searchParams).subscribe({
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
