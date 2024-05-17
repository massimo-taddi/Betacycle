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
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get('https://localhost:7287/api/Category/categories', {
      headers: header,
    });
  }
  getNProductCategories(searchParams: SearchParams): Observable<any> {
    return this.http.get(
      `https://localhost:7287/api/Category/Ncategories?PageIndex=${searchParams.pageIndex}&PageSize=${searchParams.pageSize}&Sort=${searchParams.sort}&Search=${searchParams.search}`
    );
  }
}
