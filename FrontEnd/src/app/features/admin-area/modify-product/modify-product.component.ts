import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm, NgModel } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ProductModel } from '../../../shared/models/ProductModel';
import {
  FileRemoveEvent,
  FileSelectEvent,
  FileUploadEvent,
  FileUploadModule,
} from 'primeng/fileupload';
import { CardModule } from 'primeng/card';
import { ProductForm } from '../../../shared/models/ProductForm';
import { ProductService } from '../../../shared/services/product.service';
import { CommonModule } from '@angular/common';
import * as BlobUtil from 'blob-util';
import { Product } from '../../../shared/models/Product';
import { ProductCategory } from '../../../shared/models/ProductCategory';

@Component({
  selector: 'app-modify-product',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DropdownModule,
    FileUploadModule,
    CardModule,
  ],
  templateUrl: './modify-product.component.html',
  styleUrl: './modify-product.component.css',
})
export class ModifyProductComponent {
  product: ProductForm = new ProductForm();
  categories: ProductCategory[] = [];
  models: ProductModel[] = [];
  submittedProduct: Product | null = null;
  constructor(private prodService: ProductService) {}
  modifyId: number = 0;
  ngOnInit() {
    const localStoragePosition: string | null =
      localStorage.getItem('ModifyId');

    if (localStoragePosition) {
      this.modifyId = parseInt(localStoragePosition, 10);
      this.prodService.getProductById(this.modifyId).subscribe({
        next: (product: any) => {
          this.product = product;
        },
        error: (err: Error) => {
          console.log(err.message);
        },
      });
    } else {
      console.log(
        'nessun valore è stato passato per un prodotto da modificare'
      );
    }
  }
  async onThumbnailSelect(event: FileSelectEvent) {
    BlobUtil.blobToBase64String(event.files[0]).then((b64str) => {
      this.product.thumbNailPhoto = b64str;
      this.product.thumbnailPhotoFileName = event.files[0].name;
    });
  }

  onThumbnailRemove(event: FileRemoveEvent) {
    this.product.thumbNailPhoto = null;
    this.product.thumbnailPhotoFileName = null;
  }

  SubmitProduct(newForm: NgForm) {
    var newProductForm = newForm.value as ProductForm;
    newProductForm.thumbNailPhoto = this.product.thumbNailPhoto;
    newProductForm.thumbnailPhotoFileName = this.product.thumbnailPhotoFileName;
    this.product.modifiedDate = new Date(Date.now());
    this.prodService.putProduct(newProductForm, this.modifyId).subscribe({
      next: (prod: Product) => (this.submittedProduct = prod),
      error: (err: Error) => console.log(err.message),
    });
  }
}
