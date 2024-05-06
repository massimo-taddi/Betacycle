import { Injectable } from '@angular/core';
import { Product } from '../models/Product';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { SearchParams } from '../models/SearchParams';
@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private searchParams = new BehaviorSubject(new SearchParams());
  searchParams$ = this.searchParams.asObservable();
  constructor(private http: HttpClient) {}
  getProducts(params: SearchParams): Observable<any> {
    return this.http.get(
      `https://localhost:7287/api/products?pageindex=${params.pageIndex}&pagesize=${params.pageSize}&search=${params.search}&sort=${params.sort}`
    );
  }
  setSearchParams(params: SearchParams) {
    this.searchParams.next(params);
  }

  getProductById(id: number): Observable<any> {
    return this.http.get(`https://localhost:7287/api/Products/${id}`);
  }

  postProduct(product: Product) {
    return this.http.post(`https://localhost:7287/api/Products`, product);
  }
}
