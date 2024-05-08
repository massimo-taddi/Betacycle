import { Component, OnInit } from '@angular/core';

import { ProductForm } from '../../../shared/models/ProductForm';
import { ProductService } from '../../../shared/services/product.service';
import { InputGroupModule } from 'primeng/inputgroup';

@Component({
  selector: 'app-modify-product',
  standalone: true,
  imports: [InputGroupModule],
  templateUrl: './modify-product.component.html',
  styleUrl: './modify-product.component.css',
})
export class ModifyProductComponent {
  product!: ProductForm;
  constructor(private productService: ProductService) {}
  ngOnInit() {
    const localStoragePosition: string | null =
      localStorage.getItem('ModifyId');

    if (localStoragePosition) {
      const intero: number = parseInt(localStoragePosition, 10);
      this.productService.getProductById(intero).subscribe({
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
}
