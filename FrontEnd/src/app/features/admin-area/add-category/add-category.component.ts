import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { InputSwitch, InputSwitchModule } from 'primeng/inputswitch';
import { CommonModule } from '@angular/common';
import { InputNumber, InputNumberModule } from 'primeng/inputnumber';
import { CardModule } from 'primeng/card';
import { FormsModule } from '@angular/forms';
import { ProductCategoryForm } from '../../../shared/models/ProductCategoryForm';
import { ModelService } from '../../../shared/services/model.service';
import { Router } from '@angular/router';
import { CategoryComponent } from '../category/category.component';
import { CategoryService } from '../../../shared/services/category.service';
import { DropdownModule } from 'primeng/dropdown';
import { ProductCategory } from '../../../shared/models/ProductCategory';
@Component({
  selector: 'app-add-category',
  standalone: true,
  imports: [
    ButtonModule,
    InputSwitchModule,
    CommonModule,
    InputNumberModule,
    CardModule,
    FormsModule,
    DropdownModule,
  ],
  templateUrl: './add-category.component.html',
  styleUrl: './add-category.component.css',
})
export class AddCategoryComponent {
  category: ProductCategoryForm = new ProductCategoryForm();
  categories: ProductCategory[] = [];
  constructor(private service: CategoryService, private router: Router) {}

  ngOnInit() {
    this.service.getParentProductCategories().subscribe({
      next: (categories: ProductCategory[]) => (this.categories = categories),
      error: (err: Error) => console.log(err.message),
    });
  }

  Post() {
    this.service
      .postNewProductCategory(
        this.category.parentProductCategoryId,
        this.category.name
      )
      .subscribe({
        next: (el: any) => {
          console.log('Success!');
        },
        error: (err: Error) => {
          console.log(err);
        },
      });
    this.router.navigate(['/admin/category']);
  }
}
