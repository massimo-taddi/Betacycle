import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm, NgModel } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ProductModel } from '../../../shared/models/ProductModel';
import { Router } from '@angular/router';
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
import { InputSwitch, InputSwitchModule } from 'primeng/inputswitch';
import { CategoryService } from '../../../shared/services/category.service';
import { ModelService } from '../../../shared/services/model.service';

@Component({
  selector: 'app-modify-product',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DropdownModule,
    FileUploadModule,
    CardModule,
    InputSwitchModule,
  ],
  templateUrl: './modify-product.component.html',
  styleUrl: './modify-product.component.css',
})
export class ModifyProductComponent {
  product: ProductForm = new ProductForm();
  categories: ProductCategory[] = [];
  models: ProductModel[] = [];
  updateProduct: Product | null = null;
  uploadFiles: any[] = [];
  constructor(
    private prodService: ProductService,
    private router: Router,
    private categoryService: CategoryService,
    private modelService: ModelService
  ) {}
  modifyId: number = 0;
  ngOnInit() {
    const sessionStorageProductId: string | null =
      sessionStorage.getItem('ModifyId');

    if (sessionStorageProductId) {
      this.modifyId = parseInt(sessionStorageProductId, 10);
      this.prodService.getProductById(this.modifyId).subscribe({
        next: (product: any) => {
          this.product = product;
          sessionStorage.removeItem('ModifyId');
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
    //trova tutte le categorie di prodotti
    this.categoryService.getProductCategories().subscribe({
      next: (categories: ProductCategory[]) => (this.categories = categories),
      error: (err: Error) => console.log(err.message),
    });
    //trova tutti i modelli di un prodotto
    this.modelService.getProductModels().subscribe({
      next: (models: ProductModel[]) => (this.models = models),
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

  //prende tutti i valori del form e li associa a modified product
  UpdateProduct(newForm: NgForm) {
    var modifiedProduct = newForm.value as ProductForm;
    modifiedProduct.thumbNailPhoto = this.product.thumbNailPhoto;
    modifiedProduct.thumbnailPhotoFileName =
      this.product.thumbnailPhotoFileName;
    modifiedProduct.largePhoto = this.product.largePhoto;
    modifiedProduct.largePhotoFileName = this.product.largePhotoFileName;
    this.product.modifiedDate = new Date(Date.now());

    //inviamo modifiedProduct con l'id associato

    console.log(modifiedProduct);
    this.prodService.putProduct(modifiedProduct, this.modifyId).subscribe({
      next: (prod: Product) => (this.updateProduct = prod),
      error: (err: Error) => console.log(err.message),
    });
    this.router.navigate(['/admin/products-list']);
  }
}
