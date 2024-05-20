import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../shared/services/product.service';
import { Product } from '../../shared/models/Product';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { BasketService } from '../../shared/services/basket.service';


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
  engDesc: string = '';
  justAddedProduct: Product | null = null;


  constructor(private http: ProductService, private route: ActivatedRoute, private basketService: BasketService){}


  ngOnInit(): void{
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.productId = Number(params.get('productId')!);
      this.http.getProductById(this.productId).subscribe({
        next: (data: any) =>{
          this.product = data;
          this.getEngDescription();
        },
        error: (err: Error) =>{
          console.log(err);
        }
      });
    })
  }

  private getEngDescription(){
    this.product.productModel?.productModelProductDescriptions!.forEach(desc => {
      if(desc.culture == 'en    '){
        this.engDesc = desc.productDescription!.description;
      }
    });
    return "ERROR - CANNOT GET PRODUCT DESCRIPTION";
  }

  addProductToCart() {
    this.basketService.postBasketItem(this.product).subscribe({
      next: (prod: Product) => {
        this.justAddedProduct = prod;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
}
