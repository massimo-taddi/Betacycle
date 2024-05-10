import { Component, OnInit } from '@angular/core';
import { Address } from '../../../shared/models/Address';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { FormsModule, NgForm } from '@angular/forms';
import { CustomerAddress } from '../../../shared/models/CustomerAddress';
import { AddressFormData } from '../../../shared/models/AddressFormData';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-personal-addresses',
  standalone: true,
  imports: [CommonModule ,TableModule, FormsModule, DialogModule, ButtonModule],
  templateUrl: './personal-addresses.component.html',
  styleUrl: './personal-addresses.component.css'
})
export class PersonalAddressesComponent implements OnInit {
  addresses: Address[] = [];
  dialogBoolAdd: boolean = false;
  dialogBoolEdit: boolean = false;
  dialogBoolDelete: boolean = false;
  address: Address = new Address();
  customerAddress: CustomerAddress = new CustomerAddress('');
  newAddress: AddressFormData | null = null;
  modifyAddress: Address = new Address();
  typeAddress: string = '';

  constructor(private httpAddresses: HttpUserAdminService) {}

  ngOnInit(): void {
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
  }

  PutModifyAddress(){
    //DA COMPLETARE
    this.newAddress = new AddressFormData(this.modifyAddress.addressLine1, this.modifyAddress.addressLine2,
                     this.modifyAddress.city, this.modifyAddress.stateProvince, this.modifyAddress.countryRegion,
                    this.modifyAddress.postalCode, this.typeAddress);
                    console.log(this.newAddress);
    this.httpAddresses.httpPutCustomerAddress(this.newAddress, this.modifyAddress.addressId).subscribe({
      next: (response: any) => {
        console.log(response)
      },
      error: (err: Error) => {
        console.log(err)
      }
    });
  }

  DeleteAddress(id: number){
    this.httpAddresses.httpDeleteCustomerAddress(id).subscribe();
  }
}
