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
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css',
})
export class SearchComponent {
  products!: Product[];
  searchParams: SearchParams = new SearchParams();
  constructor(private productService: ProductService) {}
  ngOnInit() {
    this.productService.searchParams$.subscribe(
      par => this.searchParams = par
    );
    this.productService
      .getProducts(this.searchParams).subscribe({
        next: (products: Product[]) => {
          this.products = products;
        },
        error: (err: Error) => {
          console.log(err.message);
        },
      });
  }
}
