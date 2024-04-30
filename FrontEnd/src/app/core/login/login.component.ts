import { Component, OnInit } from '@angular/core';
import { HttploginService } from '../../shared/services/httplogin.service';
import { LoginCredentials } from '../../shared/models/LoginCredentials';
import { HttpStatusCode } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { jwtDecode } from 'jwt-decode';

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

  constructor(private http: HttploginService, private router: Router, private authStatus: AuthenticationService) {}

  Login() {
    if (this.loginCredentials.Username != '' && this.loginCredentials.Password != '') {
      this.http.httpSendLoginCredentials(this.loginCredentials).subscribe({
        next: (response: any) => {
          switch (response.status) {
            case HttpStatusCode.Ok:
              this.jwtToken = JSON.parse(response.body).token;
              this.decodedTokenPayload = jwtDecode(this.jwtToken);
              this.authStatus.setLoginStatus(true, this.jwtToken, this.stayLoggedIn, this.decodedTokenPayload.role === 'admin');
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
}
