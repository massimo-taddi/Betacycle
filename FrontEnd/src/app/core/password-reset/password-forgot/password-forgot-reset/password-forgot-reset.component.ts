import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpUserAdminService } from '../../../../shared/services/http-user-admin.service';
import { AuthenticationService } from '../../../../shared/services/authentication.service';
import { HttploginService } from '../../../../shared/services/httplogin.service';
import { jwtDecode } from 'jwt-decode';
import { LoginCredentials } from '../../../../shared/models/LoginCredentials';
import { HttpStatusCode } from '@angular/common/http';

@Component({
  selector: 'app-password-forgot-reset',
  standalone: true,
  imports: [],
  templateUrl: './password-forgot-reset.component.html',
  styleUrl: './password-forgot-reset.component.css'
})
export class PasswordForgotResetComponent implements OnInit {
  token!: string | null;
  success: boolean = false;
  constructor(private route: ActivatedRoute, private router: Router, private resetter: HttpUserAdminService, private loginService: HttploginService, private authStatus: AuthenticationService) {}
  ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('token');
  }
  UpdateAndLogin(newPwd: HTMLInputElement) {
    this.resetter.httpTempUserResetPassword(newPwd.value, this.token!).subscribe({
      next: (response: boolean) => this.success = response,
      error: (err: Error) => {
        console.log(err.message);
      },
    });
    // adesso login:
    var decodedToken: any = jwtDecode(this.token!)
    this.loginService.httpSendLoginCredentials(new LoginCredentials(decodedToken.unique_name, newPwd.value)).subscribe({
      next: (response: any) => {
        switch (response.status) {
          case HttpStatusCode.Ok:
            this.token = JSON.parse(response.body).token;
            decodedToken = jwtDecode(this.token!);
            this.authStatus.setLoginStatus(true, this.token!, false, decodedToken.role === 'admin');
            this.router.navigate(["/home"])
            break;
          case HttpStatusCode.NoContent:
            this.success = false;
            if(response.text === 'not migrated') this.router.navigate(["/signup"]);
            break;
        }
      },
      error: (err: any) => {
        this.authStatus.setLoginStatus(false, '', false, false);
      },
    });
  }
}