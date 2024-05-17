import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { SearchParams } from '../models/SearchParams';
import { AuthenticationService } from './authentication.service';
@Injectable({
  providedIn: 'root',
})
export class ModelService {
  private searchParams = new BehaviorSubject(new SearchParams());
  searchParams$ = this.searchParams.asObservable();

  constructor(private http: HttpClient, private auth: AuthenticationService) {}

  setSearchParams(params: SearchParams) {
    this.searchParams.next(params);
  }
  getProductModels(): Observable<any> {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.get('https://localhost:7287/api/Model/models', {
      headers: header,
    });
  }

  getNProductModels(params: SearchParams): Observable<any> {
    return this.http.get(
      `https://localhost:7287/api/Model/Nmodels?PageIndex=${params.pageIndex}&PageSize=${params.pageSize}&Sort=${params.sort}`
    );
  }
}
