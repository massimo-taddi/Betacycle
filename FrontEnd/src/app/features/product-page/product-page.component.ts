import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../shared/services/product.service';
import { Product } from '../../shared/models/Product';

@Component({
  selector: 'app-product-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.css'
})
export class ProductPageComponent {
  product: Product = new Product();

  constructor(private http: ProductService){}


  ngOnInit(): void{
    this.http.getProductById(715).subscribe({
      next: (data: any) =>{
        this.product = data;
        console.log(data)
      },
      error: (err: Error) =>{
        console.log(err);
      }
    })
  }
}
