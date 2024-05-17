import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../shared/services/product.service';
import { ProductModel } from '../../../shared/models/ProductModel';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { Button, ButtonModule } from 'primeng/button';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SearchParams } from '../../../shared/models/SearchParams';
import { RouterModule } from '@angular/router';
import { ModelService } from '../../../shared/services/model.service';
@Component({
  selector: 'app-model',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    PaginatorModule,
    FormsModule,
    ButtonModule,
    RouterModule,
  ],
  templateUrl: './model.component.html',
  styleUrl: './model.component.css',
})
export class ModelComponent {
  models!: ProductModel[];
  searchParams: SearchParams = new SearchParams();
  first: number = 0;
  rows: number = 10;
  modelCount: number = 0;

  constructor(private service: ModelService) {}

  ngOnInit() {
    this.funzioneModels();
  }
  funzioneModels() {
    this.searchParams.search = '';
    this.searchParams.sort = 'Desc';
    this.service.getNProductModels(this.searchParams).subscribe({
      next: (model: any) => {
        this.modelCount = model.item1;
        this.models = model.item2;
      },
      error: (err: Error) => console.log(err.message),
    });
  }

  changeOutput(event: any) {
    this.service.searchParams$.subscribe((par) => (this.searchParams = par));
    this.searchParams.pageIndex = event.page! + 1;
    this.searchParams.pageSize = event.rows!;
    window.scroll({
      top: 0,
      left: 0,
      behavior: 'auto',
    });
    this.service.getNProductModels(this.searchParams).subscribe({
      next: (model: any) => {
        this.modelCount = model.item1;
        this.models = model.item2;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
  onPageChange(event: PaginatorState) {}
  SetLocalStorage(id: number) {
    let stringModifyId = id.toString();
    sessionStorage.setItem('ModifyModelId', stringModifyId);
  }
}
