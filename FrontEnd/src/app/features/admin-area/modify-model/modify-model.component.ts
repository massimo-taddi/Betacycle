import { Component, OnInit } from '@angular/core';
import { CardModule } from 'primeng/card';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputSwitchModule } from 'primeng/inputswitch';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Product } from '../../../shared/models/Product';
import { ProductModel } from '../../../shared/models/ProductModel';
import { FormsModule } from '@angular/forms';
import { InputText, InputTextModule } from 'primeng/inputtext';
import { ModelService } from '../../../shared/services/model.service';
import { Route, Router } from '@angular/router';

@Component({
  selector: 'app-modify-model',
  standalone: true,
  imports: [
    CardModule,
    InputNumberModule,
    InputSwitchModule,
    CommonModule,
    ButtonModule,
    FormsModule,
    InputTextModule,
  ],
  templateUrl: './modify-model.component.html',
  styleUrl: './modify-model.component.css',
})
export class ModifyModelComponent {
  model: ProductModel = new ProductModel();
  constructor(private service: ModelService, private router: Router) {}
  ngOnInit() {
    let modelIdString = sessionStorage.getItem('ModifyModelId');
    if (modelIdString) {
      let modelID = parseInt(modelIdString, 10);
      this.service.getSingleModel(modelID).subscribe({
        next: (el: any) => {
          this.model = el;
        },
        error: (err: Error) => {
          console.log(err);
        },
      });
    }
  }
  Put() {
    this.service
      .putModel(
        this.model.productModelId,
        this.model.name,
        this.model.discontinued
      )
      .subscribe({
        next: (el: any) => {
          return console.log('successo');
        },
        error: (err: Error) => {
          console.log(err);
        },
      });
    sessionStorage.removeItem('ModifyModelId');
    this.router.navigate(['/admin/model']);
  }
}
