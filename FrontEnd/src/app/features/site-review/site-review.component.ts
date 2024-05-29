import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router, RouterModule } from '@angular/router';
import { HttploginService } from '../../shared/services/httplogin.service';
import { jwtDecode } from 'jwt-decode';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { ReviewService } from '../../shared/services/review.service';
import { CustomerReview } from '../../shared/models/CustomerReview';
import { HttpStatusCode } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-site-review',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './site-review.component.html',
  styleUrl: './site-review.component.css'
})
export class SiteReviewComponent implements OnInit{
  showThanks: boolean = false;
  token: string = '';
  constructor(private router: Router,private route: ActivatedRoute, private http: HttploginService, private authStatus: AuthenticationService, private reviewService: ReviewService){}

  ngOnInit(): void {
    this.getSetTokenFromUrl();
  }

  private getSetTokenFromUrl(){
    this.token = this.route.snapshot.queryParamMap.get('token')!;
    var decodedToken: any = jwtDecode(this.token!)
    this.authStatus.setLoginStatus(true, this.token, false, decodedToken.role === 'admin');
  }

  postReview(data: HTMLTextAreaElement){
    console.log(data.value)
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
}
