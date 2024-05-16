import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { CarouselModule } from 'primeng/carousel';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { ProductService } from '../../shared/services/product.service';
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
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit{
  constructor(private http: ProductService){
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
      console.log(this.products);
    }
}
