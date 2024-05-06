import { Component, Input } from '@angular/core';
import { HttpUserAdminService } from '../../shared/services/http-user-admin.service';
import { PwResetCreds } from '../../shared/models/PwResetCreds';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { HttpHeaders } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-password-reset',
  standalone: true,
  imports: [],
  templateUrl: './password-reset.component.html',
  styleUrl: './password-reset.component.css'
})
export class PasswordResetComponent {
  result: boolean = false;
  @Input() canNotChange: boolean = false;
  constructor(private resetter: HttpUserAdminService) {}

  pwdReset(oldPwd: HTMLInputElement, newPwd: HTMLInputElement)  {
    this.resetter.httpUserResetPassword(new PwResetCreds(oldPwd.value, newPwd.value)).subscribe({
      next: (response: boolean) => this.result = response,
      error: (err: Error) => {
        console.log("Errore: "+ err.message);
      }
    });
    console.log(this.result);
  }
}
