import { Component, OnInit } from '@angular/core';
import { EmailValidator, FormsModule, NgForm } from '@angular/forms';
import { CustomerAddress } from '../../shared/models/CustomerAddress';
import { HttploginService } from '../../shared/services/httplogin.service';
import { Customer } from '../../shared/models/Customer';
import { CustomerService } from '../../shared/services/customer.service';
import { PasswordModule } from 'primeng/password'; 
import { SignUpForm } from '../../shared/models/SignUpForm';
import { InputTextModule } from 'primeng/inputtext';
import { Router } from '@angular/router';
import { BasketService } from '../../shared/services/basket.service';
import { CommonModule } from '@angular/common';
import { AuthenticationService } from '../../shared/services/authentication.service';
import { LoginCredentials } from '../../shared/models/LoginCredentials';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [FormsModule, PasswordModule, InputTextModule, CommonModule],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent implements OnInit {
  signUpSuccess: boolean = false;
  stayLoggedIn: boolean = false;
  signUpForm: SignUpForm = new SignUpForm();
  isEmailTaken: boolean = false;

  constructor(private customerService: CustomerService, private router: Router, private basketService: BasketService, private authService: AuthenticationService, private loginService: HttploginService) { }

  ngOnInit(): void {
    
  }

  SignUp(formUntyped: NgForm) {
    var form = formUntyped.value as SignUpForm;
    form.isMigrated = false;
    this.customerService.httpPostNewCustomer(form).subscribe({
      next: (response: any) => {
        this.signUpSuccess = true;
        this.loginService.httpSendLoginCredentials(new LoginCredentials(form.emailAddress, form.password)).subscribe({
          next: (response: any) => {
            this.authService.setLoginStatus(true, JSON.parse(response.body).token, this.stayLoggedIn, false);
            this.basketService.pushLocalCart();
          },
          error: (error: Error) => {
            // errore di rete
            console.log(error.message);
          }
        });
      },
      error: (error: Error) => {
        // il validate da backend non e' andato bene (o c'e' stato un errore su backend)
        console.log(error.message);
      }
      });
      this.router.navigate(["/home"]);
  }

  onBlurEmail() {
    if(this.signUpForm.emailAddress == null || this.signUpForm.emailAddress == '') return;
    this.customerService.httpIsMailTaken(this.signUpForm.emailAddress)
      .subscribe({
      next: (response: any) => {
        this.isEmailTaken = response;
      },
      error: (error: Error) => {
        // errore di rete
        console.log(error.message);
      }
      });
  }
}
