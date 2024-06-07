import { Component, OnInit } from '@angular/core';
import { HttploginService } from '../../shared/services/httplogin.service';
import { LoginCredentials } from '../../shared/models/LoginCredentials';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { jwtDecode } from 'jwt-decode';
import { BasketService } from '../../shared/services/basket.service';
import { ToastModule } from 'primeng/toast';
import { ShoppingCartItem } from '../../shared/models/ShoppingCartItem';
import { lastValueFrom, timeout } from 'rxjs';
import { Message, MessageService } from 'primeng/api';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, ToastModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  providers: [MessageService]
})
export class LoginComponent {
  loginCredentials: LoginCredentials = new LoginCredentials('','');
  jwtToken: any;
  decodedTokenPayload: any;
  stayLoggedIn: boolean = false;
  failedLogin: boolean = false;

  constructor(private http: HttploginService, private router: Router, private authStatus: AuthenticationService, private basketService: BasketService, private messageService: MessageService) {}

  Login() {
    this.failedLogin = false;
    if (this.loginCredentials.username != '' && this.loginCredentials.password != '') {
      this.http.httpSendLoginCredentials(this.loginCredentials).subscribe({
        next: (response: any) => {
          switch (response.status) {
            case HttpStatusCode.Ok:
              this.jwtToken = JSON.parse(response.body).token;
              this.decodedTokenPayload = jwtDecode(this.jwtToken);
              this.authStatus.setLoginStatus(true, this.jwtToken, this.stayLoggedIn, this.decodedTokenPayload.role === 'admin');
              // aggiungere qui i controlli e le op sul carrello
              this.basketService.pushLocalCart();
              this.router.navigate(["/home"])
              break;
            case HttpStatusCode.NoContent:
              this.failedLogin = true;
              
              break;
          }
        },
        error: (err: HttpErrorResponse) => {
          
          if(err.status == HttpStatusCode.NotFound && err.error == 'not migrated') {
            this.showLoginMigrated();
            setTimeout(() => {
              this.router.navigate(["/forgotpwd"]);
            }, 3000);
          }
          this.failedLogin = true;
          this.authStatus.setLoginStatus(false, '', false, false);

        },
      });
    } else alert('Username e Password obbligatori!');
  }

  showLoginMigrated() {
    this.messageService.add({
      severity: 'error',
      detail: 'Your account is obsolete. Please reset your password.',
    });
  }

  navToSignup() {
    this.router.navigate(['/signup']);
  }
  navToForgot() {
    this.router.navigate(['/forgotpwd']);
  }
}
