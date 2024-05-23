import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CustomerAddress } from '../../shared/models/CustomerAddress';
import { HttploginService } from '../../shared/services/httplogin.service';
import { Customer } from '../../shared/models/Customer';
import { CustomerService } from '../../shared/services/customer.service';
import { PasswordModule } from 'primeng/password'; 
import { SignUpForm } from '../../shared/models/SignUpForm';
import { InputTextModule } from 'primeng/inputtext';
import { Router } from '@angular/router';
import { BasketService } from '../../shared/services/basket.service';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [FormsModule, PasswordModule, InputTextModule],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent implements OnInit {
  signUpSuccess: boolean = false;
  stayLoggedIn: boolean = false;
  signUpForm: SignUpForm = new SignUpForm();

  constructor(private customerService: CustomerService, private router: Router, private basketService: BasketService) { }

  ngOnInit(): void {
    
  }

  SignUp(formUntyped: NgForm) {
    var form = formUntyped.value as SignUpForm;
    form.isMigrated = false;
    console.log(form);
    this.customerService.httpPostNewCustomer(form)
      .subscribe({
      next: (response: any) => {
        this.signUpSuccess = true;
      },
      error: (error: Error) => {
        // il validate da backend non e' andato bene (o c'e' stato un errore su backend)
        console.log(error.message);
      }
      });
      this.basketService.pushLocalCart();
      this.router.navigate(["/home"]);
  }
}
