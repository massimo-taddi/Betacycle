import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm, NgModel } from '@angular/forms';
import { ProductForm } from '../../../shared/models/ProductForm';
import { ProductCategory } from '../../../shared/models/ProductCategory';
import { ProductService } from '../../../shared/services/product.service';
import { DropdownModule } from 'primeng/dropdown';
import { ProductModel } from '../../../shared/models/ProductModel';
import {
  FileRemoveEvent,
  FileSelectEvent,
  FileUploadEvent,
  FileUploadModule,
} from 'primeng/fileupload';
import { Product } from '../../../shared/models/Product';
import { CardModule } from 'primeng/card';
import * as BlobUtil from 'blob-util';
import { InputSwitchModule } from 'primeng/inputswitch';
import { CategoryService } from '../../../shared/services/category.service';
import { ModelService } from '../../../shared/services/model.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DropdownModule,
    FileUploadModule,
    CardModule,
    InputSwitchModule,
  ],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.css',
})
export class AddProductComponent implements OnInit {
  product: ProductForm = new ProductForm();
  categories: ProductCategory[] = [];
  models: ProductModel[] = [];
  submittedProduct: Product | null = null;
  constructor(
    private prodService: ProductService,
    private categoryService: CategoryService,
    private modelService: ModelService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.categoryService.getProductCategories().subscribe({
      next: (categories: ProductCategory[]) => (this.categories = categories),
      error: (err: Error) => console.log(err.message),
    });
    this.modelService.getProductModels().subscribe({
      next: (models: ProductModel[]) => {
        this.models = models;
      },
      error: (err: Error) => console.log(err.message),
    });
  }

  async onThumbnailSelect(event: FileSelectEvent) {
    BlobUtil.blobToBase64String(event.files[0]).then((b64str) => {
      this.product.thumbNailPhoto = b64str;
      this.product.thumbnailPhotoFileName = event.files[0].name;
    });
  }

  async onLargeImgSelect(event: FileSelectEvent) {
    BlobUtil.blobToBase64String(event.files[0]).then((b64str) => {
      this.product.largePhoto = b64str;
      this.product.largePhotoFileName = event.files[0].name;
    });
  }

  onThumbnailRemove(event: FileRemoveEvent) {
    this.product.thumbNailPhoto = null;
    this.product.thumbnailPhotoFileName = null;
  }

  onLargeImgRemove(event: FileRemoveEvent) {
    this.product.largePhoto = null;
    this.product.largePhotoFileName = null;
  }

  SubmitProduct(newForm: NgForm) {
    var newProductForm = newForm.value as ProductForm;
    newProductForm.thumbNailPhoto = this.product.thumbNailPhoto;
    newProductForm.thumbnailPhotoFileName = this.product.thumbnailPhotoFileName;
    newProductForm.largePhoto = this.product.largePhoto;
    newProductForm.largePhotoFileName = this.product.largePhotoFileName;
    this.product.modifiedDate = new Date(Date.now());
    this.prodService.postProduct(newProductForm).subscribe({
      next: (prod: Product) => (this.submittedProduct = prod),
      error: (err: Error) => console.log(err.message),
    });
    this.router.navigate(['/admin/products-list']);
  }
}
