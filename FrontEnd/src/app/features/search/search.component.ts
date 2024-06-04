import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { SidebarModule } from 'primeng/sidebar';
import { FormsModule } from '@angular/forms';
import { RatingModule } from 'primeng/rating';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { Product } from '../../shared/models/Product';
import { ProductService } from '../../shared/services/product.service';
import { CommonModule } from '@angular/common';
import { SearchParams } from '../../shared/models/SearchParams';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { BasketService } from '../../shared/services/basket.service';
import { Router, RouterModule } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { AuthenticationService } from '../../shared/services/authentication.service';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    SidebarModule,
    FormsModule,
    DataViewModule,
    RatingModule,
    TagModule,
    CommonModule,
    PaginatorModule,
    ProgressSpinnerModule,
    RouterModule,
    ToastModule,
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css',
  providers: [MessageService],
})
export class SearchComponent implements OnInit {
  products: Product[] | null = null;
  searchParams: SearchParams = new SearchParams();
  productCount: number = 0;
  justAddedProduct: Product | null = null;
  isUserAdmin: boolean = false;
  isUserLoggedIn: boolean = false;
  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private basketService: BasketService,
    private router: Router,
    private messageService: MessageService,
    private authenticationService: AuthenticationService
  ) {}

  ngOnInit(): void {
    this.authenticationService.isLoggedIn$.subscribe(
      (res) => (this.isUserLoggedIn = res)
    );
    if (localStorage.getItem('jwtToken') != null)
      sessionStorage.setItem('jwtToken', localStorage.getItem('jwtToken')!);
    this.authenticationService.isAdmin$.subscribe(
      (res) => (this.isUserAdmin = res)
    );
    this.getParamsFromUrl();
  }

  changeOutput(event: PaginatorState) {
    this.searchParams.pageIndex = event.page! + 1;
    this.searchParams.pageSize = event.rows!;
    this.productService.searchParams$.subscribe(
      (par) => (this.searchParams = par)
    );
    this.products = null;
    this.productService.getProducts(this.searchParams).subscribe({
      next: (products: any) => {
        this.products = products.item2;
        this.productCount = products.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
  private fillProducts() {
    this.productService.setSearchParams(this.searchParams);
    this.products = null;
    if (this.searchParams.search == 'all') {
      this.searchParams.search = '';
      this.productService.getProducts(this.searchParams).subscribe({
        next: (products: any) => {
          this.products = products.item2;
          this.productCount = products.item1;
        },
        error: (err: Error) => {
          console.log(err.message);
        },
      });
    } else {
      this.productService.getProducts(this.searchParams).subscribe({
        next: (products: any) => {
          this.products = products.item2;
          this.productCount = products.item1;
        },
        error: (err: Error) => {
          console.log(err.message);
        },
      });
    }
  }
  private getParamsFromUrl() {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.searchParams.search = params.get('search')!;
      this.searchParams.pageIndex = +params.get('pageIndex')!;
      this.searchParams.pageSize = +params.get('pageSize')!;
      this.searchParams.sort = params.get('sort')!;
      this.fillProducts();
    });
  }

  addProductToCart(product: Product) {
    this.basketService.postBasketItem(product).subscribe({
      next: (prod: Product) => {
        this.justAddedProduct = prod;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
    this.showAdded();
  }

  productDetails(id: number) {
    this.router.navigate(['/product-page', id]);
  }

  showAdded() {
    this.messageService.add({
      severity: 'success',
      detail: 'Added product to cart',
    });
  }

  showError() {
    this.messageService.add({
      severity: 'error',
      detail: 'This product is already in your cart',
    });
  }
}
