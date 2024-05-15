import { Injectable } from '@angular/core';
import { Product } from '../models/Product';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { SearchParams } from '../models/SearchParams';
import { ProductForm } from '../models/ProductForm';
import { AuthenticationService } from './authentication.service';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';
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
    return this.http.get(
      `https://localhost:7287/api/products?pageindex=${params.pageIndex}&pagesize=${params.pageSize}&search=${params.search}&sort=${params.sort}`
    );
  }

  getProductCategories(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get('https://localhost:7287/api/products/categories', {
      headers: header,
    });
  }

  getNProductCategories(searchParams: SearchParams): Observable<any> {
    return this.http.get(
      `https://localhost:7287/api/Products/Ncategories?PageIndex=${searchParams.pageIndex}&PageSize=${searchParams.pageSize}&Sort=${searchParams.sort}&Search=${searchParams.search}`
    );
  }

  getProductModels(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get('https://localhost:7287/api/products/models', {
      headers: header,
    });
  }
  //work in progress
  getNProductModels(params: SearchParams): Observable<any> {
    return this.http.get(
      `https://localhost:7287/api/Products/Nmodels?PageIndex=${params.pageIndex}&PageSize=${params.pageSize}&Sort=${params.sort}`
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
}
