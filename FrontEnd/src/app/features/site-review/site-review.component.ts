import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router, RouterModule } from '@angular/router';
import { HttploginService } from '../../shared/services/httplogin.service';
import { jwtDecode } from 'jwt-decode';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { ReviewService } from '../../shared/services/review.service';
import { CustomerReview } from '../../shared/models/CustomerReview';
import { HttpStatusCode } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounce, debounceTime, interval, scan } from 'rxjs';
import { RatingModule } from 'primeng/rating';

@Component({
  selector: 'app-site-review',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule, RatingModule],
  templateUrl: './site-review.component.html',
  styleUrl: './site-review.component.css'
})
export class SiteReviewComponent implements OnInit, OnDestroy{
  showThanks: boolean = false;
  token: string = '';
  reviewText: string = '';
  reviewText$: Subject<string> = new Subject<string>();
  reviewScore: number = 1;
  roundedReviewScore: number = 1;
  constructor(private route: ActivatedRoute, private authStatus: AuthenticationService, private reviewService: ReviewService){}

  ngOnInit(): void {
    this.getSetTokenFromUrl();
    this.reviewText$
      .pipe(debounceTime(300))
      .subscribe({
      next: (nextRev: string) => {
        this.getReviewScore(nextRev);
      }
    });
  }

  ngOnDestroy(): void {
    this.reviewText$.complete();
  }

  private getSetTokenFromUrl(){
    this.token = this.route.snapshot.queryParamMap.get('token')!;
    var decodedToken: any = jwtDecode(this.token!)
    this.authStatus.setLoginStatus(true, this.token, false, decodedToken.role === 'admin');
  }

  postReview(data: HTMLTextAreaElement){
    var review = {
      reviewId: 0,
      bodyDescription: data.value,
      rating: '0',
      reviewDate: new Date(Date.now()),
      modifiedDate: new Date(Date.now())
    } as CustomerReview;
    this.reviewService.httpPostReview(review).subscribe(
      {
        next: (resp: any) =>{
          if(HttpStatusCode.Ok)
            this.showThanks = true;
        },
        error: (err: Error) => {
          console.log(err);
        }
      }
    );
  }

  getReviewScore(reviewText: string) {
    this.reviewService.httpGetReviewScore(reviewText).subscribe({
      next: (resp: number) => {
        this.reviewScore = resp;
        this.roundedReviewScore = Math.round(this.reviewScore);
      },
      error: (err: Error) => {
        console.log(err);
      }
    });
  }

  onTextAreaChanges(reviewText: HTMLTextAreaElement){
    this.reviewText$.next(reviewText.value);
  }
}
