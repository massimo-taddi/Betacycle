import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm, NgModel } from '@angular/forms';
import { ProductForm } from '../../../shared/models/ProductForm';
import { ProductCategory } from '../../../shared/models/ProductCategory';
import { ProductService } from '../../../shared/services/product.service';
import { DropdownModule } from 'primeng/dropdown';
import { ProductModel } from '../../../shared/models/ProductModel';
import { FileRemoveEvent, FileSelectEvent, FileUploadEvent, FileUploadModule } from 'primeng/fileupload';
import { Product } from '../../../shared/models/Product';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [CommonModule, FormsModule, DropdownModule, FileUploadModule, CardModule],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.css',
})
export class AddProductComponent implements OnInit {
  product: ProductForm = new ProductForm();
  categories: ProductCategory[] = [];
  models: ProductModel[] = [];
  submittedProduct: Product | null = null;
  constructor(private prodService: ProductService) {}

  ngOnInit(): void {
    this.prodService.getProductCategories().subscribe({
      next: (categories: ProductCategory[]) => this.categories = categories,
      error: (err: Error) => console.log(err.message)
    });
    this.prodService.getProductModels().subscribe({
      next: (models: ProductModel[]) => this.models = models,
      error: (err: Error) => console.log(err.message)
    });
  }

  async onThumbnailSelect(event: FileSelectEvent) {
    this.product.thumbNailPhoto = await event.files[0].text();
    this.product.thumbnailPhotoFileName = event.files[0].name;
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
    this.prodService.postProduct(newProductForm).subscribe({
      next: (prod: Product) => this.submittedProduct = prod,
      error: (err: Error) => console.log(err.message)
    });
  }
}
