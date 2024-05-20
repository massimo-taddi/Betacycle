import { Component, OnInit, input } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ProductModel } from '../../../shared/models/ProductModel';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { ProductCategoryForm } from '../../../shared/models/ProductCategoryForm';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { InputSwitchModule } from 'primeng/inputswitch';
import { CategoryService } from '../../../shared/services/category.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-modify-category',
  standalone: true,
  imports: [
    CardModule,
    ButtonModule,
    FormsModule,
    InputNumberModule,
    InputTextModule,
    InputSwitchModule,
  ],
  templateUrl: './modify-category.component.html',
  styleUrl: './modify-category.component.css',
})
export class ModifyCategoryComponent {
  category: ProductCategoryForm = new ProductCategoryForm();

  constructor(private service: CategoryService, private router: Router) {}
  ngOnInit() {
    let idDaCercareStringa = sessionStorage.getItem('ModifyIdCategory');
    if (idDaCercareStringa != null) {
      let idDaCercare = parseInt(idDaCercareStringa, 10);
      this.service.getSingleCategory(idDaCercare).subscribe({
        next: (category: any) => {
          this.category = category;
          sessionStorage.removeItem('ModifyIdCategory');
        },
        error: (err: Error) => {
          console.log(err.message);
        },
      });
    }
  }
  Update() {
    this.service
      .updateCategory(
        this.category.productCategoryId,
        this.category.name,
        this.category.discontinued,
        this.category.parentProductCategoryId
      )
      .subscribe({
        next: () => {
          return console.log('success');
        },
        error: (err: Error) => {
          console.log(err);
        },
      });
    this.router.navigate(['/admin/category']);
  }
}
