import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthenticationService } from './authentication.service';
import { CustomerReview } from '../models/CustomerReview';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(private http: HttpClient, private auth: AuthenticationService) { }

  httpPostReview(review: CustomerReview): Observable<any>{
    var header = new HttpHeaders();
    this.auth.authJwtHeader$.subscribe((h) => (header = h));
    return this.http.post('https://localhost:7287/api/CustomerReviews', review, {
      headers: header,
    });
  }

  httpGetReviews(): Observable<any>{
    return this.http.get('https://localhost:7287/api/CustomerReviews');
  }

  httpGetReviewScore(reviewText: string): Observable<any>{
    return this.http.post('https://localhost:7287/api/CustomerReviews/GetReviewScore', `"${reviewText}"`, {headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' },});
  }
}
