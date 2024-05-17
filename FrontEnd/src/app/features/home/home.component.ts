import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { CarouselModule } from 'primeng/carousel';
import { ButtonModule } from 'primeng/button';
import { Router, RouterModule } from '@angular/router';
import { ScrollTopModule } from 'primeng/scrolltop';import { ProductService } from '../../shared/services/product.service';
import { Product } from '../../shared/models/Product';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TagModule,
    CarouselModule,
    ButtonModule,
    RouterModule,
    ScrollTopModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit{
  constructor(private http: ProductService, private router: Router){
  }
  responsiveOptions : any[]=[
    {
        breakpoint: '--bs-breakpoint-lg',
        numVisible: 2,
        numScroll: 3
    }]
    products: Product[] =[];
    i: number =0;

    ngOnInit(): void{
      this.FillProducts()
    }

    FillProducts(){
      //TEST DEL CAROUSEL CON FOR
      for(this.i=708;this.i< 717;this.i++){
        this.http.getProductById(this.i).subscribe({
          next: (data: any) =>{
            this.products.push(data);
          }
        });
      }
      // this.http.getRecommendedProducts().subscribe({
      //   next: (data: any) =>{
      //     this.products = data;
      //   },
      //   error: (err: Error) =>{
      //     console.log(err);        }
      // })
    }
    productDetails(id: number){
      this.router.navigate(['/product-page', id]);
    }
}
