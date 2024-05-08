import { Component, OnInit } from '@angular/core';
import { Address } from '../../../shared/models/Address';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { FormsModule, NgForm } from '@angular/forms';
import { CustomerAddress } from '../../../shared/models/CustomerAddress';
import { AddressFormData } from '../../../shared/models/AddressFormData';

@Component({
  selector: 'app-personal-addresses',
  standalone: true,
  imports: [CommonModule ,TableModule, FormsModule, DialogModule],
  templateUrl: './personal-addresses.component.html',
  styleUrl: './personal-addresses.component.css'
})
export class PersonalAddressesComponent implements OnInit {
  addresses: Address[] = [];
  dialogBool: boolean = false;
  address: Address = new Address();
  customerAddress: CustomerAddress = new CustomerAddress('');
  newAddress: AddressFormData | null = null;

  constructor(private httpAddresses: HttpUserAdminService) {}

  ngOnInit(): void {
    this.addresses = [];
    this.getUserAddresses();
  }

  private getUserAddresses() {
    this.httpAddresses.httpGetCustomerAddresses().subscribe({
      next: (addressList: Address[]) => {
        this.addresses = addressList
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }

  SubmitAddress(newForm: NgForm){
    this.newAddress = newForm.value as AddressFormData;
    this.httpAddresses.httpPostCustomerAddress(this.newAddress).subscribe();
    this.addresses.push(this.address);
  }
}
