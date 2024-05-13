import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../shared/services/product.service';
import { ProductModel } from '../../../shared/models/ProductModel';
import { NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-model',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './model.component.html',
  styleUrl: './model.component.css',
})
export class ModelComponent {
  models!: ProductModel[];
  constructor(private service: ProductService) {}
  ngOnInit() {
    this.service.getProductModels().subscribe({
      next: (models: ProductModel[]) => (this.models = models),
      error: (err: Error) => console.log(err.message),
    });
  }
}
