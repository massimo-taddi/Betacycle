import { Component, OnInit } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ProductModel } from '../../../shared/models/ProductModel';

@Component({
  selector: 'app-modify-category',
  standalone: true,
  imports: [CardModule, ButtonModule],
  templateUrl: './modify-category.component.html',
  styleUrl: './modify-category.component.css',
})
export class ModifyCategoryComponent {
  model: ProductModel = new ProductModel();
  ngOnInit() {}
  Update() {
    console.log('updateWorks');
  }
}
