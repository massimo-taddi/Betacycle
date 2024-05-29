import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { CarouselModule } from 'primeng/carousel';
import { ButtonModule } from 'primeng/button';
import { Router, RouterModule } from '@angular/router';
import { ScrollTopModule } from 'primeng/scrolltop';import { ProductService } from '../../shared/services/product.service';
import { Product } from '../../shared/models/Product';
import { LoginComponent } from '../../core/login/login.component';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { ReviewService } from '../../shared/services/review.service';
import { ReviewDataForm } from '../../shared/models/ReviewDataForm';
import { lastValueFrom } from 'rxjs';
import {RatingModule} from 'primeng/rating';

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
    ScrollTopModule,
    LoginComponent,
    RatingModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit{
  isUserLoggedIn: boolean = false;
  isUserAdmin: boolean = false;
  products: Product[] =[];
  reviews: ReviewDataForm [] = [];


  constructor(private http: ProductService, private router: Router, private authenticationService: AuthenticationService, private reviewSvc: ReviewService){
  }
  responsiveOptions : any[]=[
    {
        breakpoint: getComputedStyle(document.body)
        .getPropertyValue('--bs-breakpoint-lg')
        .trim(),
        numVisible: 2,
        numScroll: 2
    },
    {
      breakpoint: getComputedStyle(document.body)
      .getPropertyValue('--bs-breakpoint-sm')
      .trim(),
      numVisible: 1,
      numScroll: 1
  }
  ]
    

    ngOnInit(): void{
      this.authenticationService.isLoggedIn$.subscribe(
        (res) => (this.isUserLoggedIn = res)
      );
      if (localStorage.getItem('jwtToken') != null)
        sessionStorage.setItem('jwtToken', localStorage.getItem('jwtToken')!);
      this.authenticationService.isAdmin$.subscribe(
        (res) => (this.isUserAdmin = res)
      );
      if(this.isUserLoggedIn)
        this.FillRecommendProducts();
      else
        this.FillRandProducts();
      this.getRandomReviews();
    }

    private async getRandomReviews(){
      this.reviews = await lastValueFrom(this.reviewSvc.httpGetReviews());
      console.log(this.reviews)
    }

    FillRecommendProducts(){
      this.http.getRecommendedProducts().subscribe({
        next: (data: any) =>{
          this.products = data;
          console.log(data)
        },
        error: (err: Error) =>{
          console.log(err);        }
      })
    }
    ProductDetails(id: number){
      this.router.navigate(['/product-page', id]);
    }

    FillRandProducts(){
      this.http.getRandomProducts().subscribe({
        next: (data: any) =>{
          this.products = data;
          console.log(data)
        },
        error: (err: Error) =>{
          console.log(err);        }
      })
    }
}
