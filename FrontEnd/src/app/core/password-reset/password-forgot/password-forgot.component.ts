import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';

@Component({
  selector: 'app-password-forgot',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './password-forgot.component.html',
  styleUrl: './password-forgot.component.css'
})
export class PasswordForgotComponent {
  emailSent: boolean = false;
  constructor(private resetter: HttpUserAdminService, private router: Router) {

  }
  SendMail(email: HTMLInputElement) {
    this.resetter.httpSendResetEmail(email.value).subscribe(
      res => this.emailSent = res
    );
    this.router.navigate(['/home']);
  }
}
