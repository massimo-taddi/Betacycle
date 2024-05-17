import { Component, OnInit } from '@angular/core';
import { PaginatorModule } from 'primeng/paginator';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SearchParams } from '../../../shared/models/SearchParams';
import { ProductCategory } from '../../../shared/models/ProductCategory';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../../shared/services/product.service';
import { CategoryService } from '../../../shared/services/category.service';
@Component({
  selector: 'app-category',
  standalone: true,
  imports: [
    PaginatorModule,
    CardModule,
    FormsModule,
    ButtonModule,
    CommonModule,
    RouterModule,
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

  constructor(private service: CategoryService) {}
  ngOnInit() {
    this.ShowCategories();
  }
  ShowCategories() {
    //modificare la chiamata a controller
    this.searchParams.search = '';
    this.searchParams.sort = 'Desc';
    this.service.getNProductCategories(this.searchParams).subscribe({
      next: (category: any) => {
        this.categories = category.item2;
        this.categoriesCount = category.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }

  changeOutput(event: any) {
    this.searchParams.search = '';
    this.searchParams.sort = 'Desc';

    this.searchParams.pageIndex = event.page! + 1;
    this.searchParams.pageSize = event.rows!;

    window.scroll({
      top: 0,
      left: 0,
      behavior: 'auto',
    });
    //deve essere un get NCategory
    this.service.getNProductCategories(this.searchParams).subscribe({
      next: (category: any) => {
        console.log(category);
        this.categories = category.item2;
        this.categoriesCount = category.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
  NavigateModify(categoryId: number) {
    let stringCategoryID = categoryId.toString();

    sessionStorage.setItem('ModifyIdCategory', stringCategoryID);
  }
}
