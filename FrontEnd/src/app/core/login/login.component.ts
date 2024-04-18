import { Component } from '@angular/core';
import { HttploginService } from '../../shared/services/httplogin.service';
import { LoginCredentials } from '../../shared/models/LoginCredentials';
import { HttpStatusCode } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginResponseBody: string = '';
  loginResponseStatus: HttpStatusCode = HttpStatusCode.NotFound;
  constructor(private httpLogin: HttploginService) {}
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
              console.log("not migrated");
              break;
            case "migrated":
              // fa il login
              localStorage.setItem('credentials', Username.value + ":" + Password.value);
              console.log("migrated");
              break;
            case "admin":
              // fa il login e appare un'opzione extra nella navbar x area amministratore
              localStorage.setItem('credentials', Username.value + ":" + Password.value);
              console.log("admin");
              break;
            case "nope":
              console.log("vedi");
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
