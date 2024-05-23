import { Injectable } from '@angular/core';
import { Product } from '../models/Product';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { SearchParams } from '../models/SearchParams';
import { ProductForm } from '../models/ProductForm';
import { AuthenticationService } from './authentication.service';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';
import { PaginatorParams } from '../models/PaginatorParams';
@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private searchParams = new BehaviorSubject(new SearchParams());
  searchParams$ = this.searchParams.asObservable();

  constructor(private http: HttpClient, private auth: AuthenticationService) {}

  setSearchParams(params: SearchParams) {
    this.searchParams.next(params);
  }

  getProducts(params: SearchParams): Observable<any> {
    if (params.search != '') {
      return this.http.get(
        `https://localhost:7287/api/Products?PageIndex=${params.pageIndex}&PageSize=${params.pageSize}&Search=${params.search}&Sort=${params.sort}`
      );
    } else {
      return this.getAllProducts(params);
    }
  }
  getAllProducts(params: PaginatorParams): Observable<any> {
    return this.http.get(
      `https://localhost:7287/api/Products/GetAllProducts?PageIndex=${params.pageIndex}&PageSize=${params.pageSize}&Sort=${params.sort}`
    );
  }
  getProductById(id: number): Observable<any> {
    return this.http.get(`https://localhost:7287/api/Products/${id}`);
  }

  postProduct(productForm: ProductForm): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post(`https://localhost:7287/api/Products`, productForm, {
      headers: header,
    });
  }
  putProduct(productForm: ProductForm, id: number): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.put(
      `https://localhost:7287/api/Products/${id}`,
      productForm,
      {
        headers: header,
      }
    );
  }

  getRecommendedProducts(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get(
      `https://localhost:7287/api/Products/Recommendations`,
      {
        headers: header,
      }
    );
  }

  getRandomProducts(): Observable<any> {
    return this.http.get(`https://localhost:7287/api/products/randomproducts`);
  }
}
