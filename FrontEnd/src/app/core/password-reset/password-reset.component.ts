import { Component } from '@angular/core';
import { HttpUserAdminService } from '../../shared/services/http-user-admin.service';
import { PwResetCreds } from '../../shared/models/PwResetCreds';

@Component({
  selector: 'app-password-reset',
  standalone: true,
  imports: [],
  templateUrl: './password-reset.component.html',
  styleUrl: './password-reset.component.css'
})
export class PasswordResetComponent {
  res: boolean = false;
  constructor(private resetter: HttpUserAdminService) {}

  pwdReset(oldPwd: HTMLInputElement, newPwd: HTMLInputElement)  {
    this.resetter.httpUserResetPassword(new PwResetCreds(oldPwd.value, newPwd.value)).subscribe({
      next: (response: any) => this.res = response,
      error: (err: Error) => {
        console.log("Errore: "+ err.message);
      }
    });
    console.log(this.res);
  }
}
