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
    return this.http.get('https://localhost:7287/api/Model', {
      headers: header,
    });
  }

  getNProductModels(params: SearchParams): Observable<any> {
    if (params.search == '') {
      return this.http.get(
        `https://localhost:7287/api/Model/Nmodels?PageIndex=${params.pageIndex}&PageSize=${params.pageSize}&Sort=${params.sort}`
      );
    } else {
      return this.http.get(
        `https://localhost:7287/api/Model/Nmodels?Search=${params.search}&PageIndex=${params.pageIndex}&PageSize=${params.pageSize}&Sort=${params.sort}`
      );
    }
  }
  getSingleModel(id: number) {
    return this.http.get(`https://localhost:7287/api/Model/${id}`);
  }
  putModel(id: number, name: string, discontinued: boolean) {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.put(
      `https://localhost:7287/api/Model/${id}?name=${name}&discontinued=${discontinued}`,
      {},
      { headers: header }
    );
  }
  postModel(name: string, discontinued: boolean) {
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post(
      `https://localhost:7287/api/Model?name=${name}&discontinued=${discontinued}`,
      {},
      { headers: header }
    );
  }
}
