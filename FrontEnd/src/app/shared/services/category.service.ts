import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { SearchParams } from '../models/SearchParams';
import { AuthenticationService } from './authentication.service';
@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private searchParams = new BehaviorSubject(new SearchParams());
  searchParams$ = this.searchParams.asObservable();

  constructor(private http: HttpClient, private auth: AuthenticationService) {}

  setSearchParams(params: SearchParams) {
    this.searchParams.next(params);
  }
  getProductCategories(): Observable<any> {
    return this.http.get('https://localhost:7287/api/Category');
  }
  getParentProductCategories(): Observable<any> {
    return this.http.get(
      'https://localhost:7287/api/Category/ParentCategories'
    );
  }
  getNProductCategories(searchParams: SearchParams): Observable<any> {
    return this.http.get(
      `https://localhost:7287/api/Category/Ncategories?PageIndex=${searchParams.pageIndex}&PageSize=${searchParams.pageSize}&Sort=${searchParams.sort}&Search=${searchParams.search}`
    );
  }
  postNewProductCategory(
    parentCategory: number,
    categoryName: string
  ): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    if (parentCategory != 0) {
      return this.http.post(
        `https://localhost:7287/api/Category?parentProductCategory=${parentCategory}&name=${categoryName}`,
        {},
        { headers: header }
      );
    } else {
      return this.http.post(
        `https://localhost:7287/api/Category?name=${categoryName}`,
        {},
        { headers: header }
      );
    }
  }
  getSingleCategory(id: number): Observable<any> {
    return this.http.get(`https://localhost:7287/api/Category/${id}`);
  }
  updateCategory(
    id: number,
    name: string,
    discontinued: boolean,
    parentCategory: number
  ): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    if (parentCategory != null) {
      return this.http.put(
        `https://localhost:7287/api/Category/${id}?name=${name}&discontinued=${discontinued}&parentCategory=${parentCategory}`,
        {},
        { headers: header }
      );
    } else {
      return this.http.put(
        `https://localhost:7287/api/Category/${id}?name=${name}&discontinued=${discontinued}`,
        {},
        { headers: header }
      );
    }
  }
  deleteCategory(id: number) {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.delete(`https://localhost:7287/api/Category/${id}`, {
      headers: header,
    });
  }
}
