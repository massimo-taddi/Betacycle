import { Component, OnInit } from '@angular/core';
import { HttploginService } from '../../shared/services/httplogin.service';
import { LoginCredentials } from '../../shared/models/LoginCredentials';
import { HttpStatusCode } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { jwtDecode } from 'jwt-decode';
import { BasketService } from '../../shared/services/basket.service';
import { ShoppingCartItem } from '../../shared/models/ShoppingCartItem';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginCredentials: LoginCredentials = new LoginCredentials('','');
  jwtToken: any;
  decodedTokenPayload: any;
  stayLoggedIn: boolean = false;
  failedLogin: boolean = false;

  constructor(private http: HttploginService, private router: Router, private authStatus: AuthenticationService, private basketService: BasketService) {}

  Login() {
    if (this.loginCredentials.username != '' && this.loginCredentials.password != '') {
      this.http.httpSendLoginCredentials(this.loginCredentials).subscribe({
        next: (response: any) => {
          switch (response.status) {
            case HttpStatusCode.Ok:
              this.jwtToken = JSON.parse(response.body).token;
              this.decodedTokenPayload = jwtDecode(this.jwtToken);
              this.authStatus.setLoginStatus(true, this.jwtToken, this.stayLoggedIn, this.decodedTokenPayload.role === 'admin');
              // aggiungere qui i controlli e le op sul carrello
              this.pushLocalCart();
              this.router.navigate(["/home"])
              break;
            case HttpStatusCode.NoContent:
              this.failedLogin = true;
              if(response.text === 'not migrated') this.router.navigate(["/signup"]);
              break;
          }
        },
        error: (err: any) => {
          this.authStatus.setLoginStatus(false, '', false, false);
        },
      });
    } else alert('Username e Password obbligatori!');
  }

  pushLocalCart() {
    var localBasket = localStorage.getItem('basket');
    if(localBasket != undefined) { //user non loggato ha il basket in local
      var localBasketFound = JSON.parse(localBasket) as ShoppingCartItem[];
      this.basketService.userHasBasket().subscribe((response: boolean) => {
        if(!response) { //basket in local presente ma non sul db
          this.basketService.postBasketItemRemote(localBasketFound[0]!, true).subscribe({
            next: (resp: any) =>{
              if(resp != null && localBasketFound.length > 1){
                for(var i=1; i< localBasketFound.length; i++){
                  this.basketService.postBasketItemRemote(localBasketFound[i], true).subscribe();
                }
              }
            },
            error: (err: Error) =>{
              console.log(err)
            }
          });
        }
        localStorage.removeItem('basket');
      });
    }
  }

  navToSignup() {
    this.router.navigate(['/signup']);
  }
  navToForgot() {
    this.router.navigate(['/forgotpwd']);
  }
}
