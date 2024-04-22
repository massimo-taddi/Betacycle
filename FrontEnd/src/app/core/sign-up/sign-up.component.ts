import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CustomerAddress } from '../../shared/models/CustomerAddress';
import { HttploginService } from '../../shared/services/httplogin.service';
import { Customer } from '../../shared/models/Customer';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent implements OnInit {
  signUpSuccess: boolean = false;
  stayLoggedIn: boolean = false;

  constructor(private httpService: HttploginService) { }

  ngOnInit(): void {
    
  }

  SignUp(
    Title: string,
    FirstName: string,
    MiddleName: string,
    LastName: string,
    Suffix: string,
    CompanyName: string,
    SalesPerson: string,
    EmailAddress: string,
    Phone: string,
    Password: string,
    IsMigrated: boolean,
    CustomerAddresses: CustomerAddress[]
  ) {
    this.httpService.httpPostNewCustomer(new Customer(Title, FirstName, MiddleName, LastName, Suffix, CompanyName, SalesPerson, EmailAddress, Phone, Password, new Date(Date.now()), IsMigrated, CustomerAddresses))
      .subscribe({
        next: (response: any) => {
          this.signUpSuccess = true;
        },
        error: (error: Error) => {
          // il validate da backend non e' andato bene (o c'e' stato un errore su backend)
          console.log(error.message);
        }
      });
  }
}
