import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../shared/services/product.service';
import { Product } from '../../shared/models/Product';
import { ActivatedRoute, ParamMap } from '@angular/router';


@Component({
  selector: 'app-product-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.css'
})
export class ProductPageComponent {
  product: Product = new Product();
  productId: number = 0;

  constructor(private http: ProductService, private route: ActivatedRoute,){}


  ngOnInit(): void{
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.productId = Number(params.get('productId')!);
      this.http.getProductById(this.productId).subscribe({
        next: (data: any) =>{
          this.product = data;
          console.log(data)
        },
        error: (err: Error) =>{
          console.log(err);
        }
      });
    })
  }
}
