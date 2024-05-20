import { Component } from '@angular/core';
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
  ],
  templateUrl: './add-category.component.html',
  styleUrl: './add-category.component.css',
})
export class AddCategoryComponent {
  category: ProductCategoryForm = new ProductCategoryForm();

  constructor(private service: CategoryService, private router: Router) {}

  Post() {
    this.service
      .postNewProductCategory(
        this.category.parentProductCategoryId,
        this.category.name
      )
      .subscribe({
        next: (el: any) => {
          console.log('Post avvenuta con successo');
        },
        error: (err: Error) => {
          console.log(err);
        },
      });
    this.router.navigate(['/admin/category']);
  }
}
