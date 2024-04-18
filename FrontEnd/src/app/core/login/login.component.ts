import { Component, OnInit } from '@angular/core';
import { HttploginService } from '../../shared/services/httplogin.service';
import { LoginCredentials } from '../../shared/models/LoginCredentials';
import { HttpStatusCode } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginStatusService } from '../../shared/services/login-status.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit{
  loginResponseBody: string = '';
  loginResponseStatus: HttpStatusCode = HttpStatusCode.NotFound;
  stayLoggedIn: boolean = false;
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;

  constructor(private httpLogin: HttploginService, private router: Router, private route: ActivatedRoute, private loggedInStatus: LoginStatusService) {}

  ngOnInit(): void {
    this.loggedInStatus.isLoggedIn$.subscribe(
      res => this.isLoggedIn = res
    );
    localStorage.getItem('isLoggedIn') === 'true' || sessionStorage.getItem('isLoggedIn') === 'true' ? this.isLoggedIn = true : this.isLoggedIn = false;
    localStorage.getItem('isAdmin') === 'true' || sessionStorage.getItem('isAdmin') === 'true' ? this.isAdmin = true : this.isAdmin = false;
  }

  Login(Username: HTMLInputElement, Password: HTMLInputElement) {
    this.httpLogin.httpSendLoginCredentials(new LoginCredentials(Username.value, Password.value)).subscribe({
      next: (response: any) => {
        this.loginResponseStatus = response.status;
        this.loginResponseBody = response.body;
        if(this.loginResponseStatus === HttpStatusCode.Ok) {
          // TODO: add criptaggio con window.btoa
          switch(this.loginResponseBody) {
            case "notMigrated":
              // porta utente a una pagina di registrazione ("abbiamo effettuato una migrazione")
              this.router.navigate(["/signup"]);
              break;
            case "migrated":
              // fa il login e appare l'area personale
              if(this.stayLoggedIn)
                localStorage.setItem('credentials', window.btoa(Username.value + ":" + Password.value));
              else
                sessionStorage.setItem('credentials', window.btoa(Username.value + ":" + Password.value));
              this.loggedInStatus.setLoggedIn(true, this.stayLoggedIn);
              this.router.navigate(["/home"]);
              break;
            case "admin":
              // fa il login e appare un'opzione extra nella navbar x area amministratore
              if(this.stayLoggedIn)
                localStorage.setItem('credentials', window.btoa(Username.value + ":" + Password.value));
              else
                sessionStorage.setItem('credentials', window.btoa(Username.value + ":" + Password.value));
              this.loggedInStatus.setLoggedIn(true, this.stayLoggedIn);
              this.loggedInStatus.setAdmin(true, this.stayLoggedIn);
              this.router.navigate(["/home"]);
              break;
          }
        }
      },
      error: (error: any) => {
        if(error.status === HttpStatusCode.NotFound) {
          console.log("not found")
        }
      }
    });
  }
}
