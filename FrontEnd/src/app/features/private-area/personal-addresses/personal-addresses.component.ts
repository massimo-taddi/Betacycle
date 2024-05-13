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
import { HttpStatusCode } from '@angular/common/http';
import {MessageService} from 'primeng/api';
import { PrimeNGConfig } from 'primeng/api';
import { ToastModule } from 'primeng/toast';


@Component({
  selector: 'app-personal-addresses',
  standalone: true,
  imports: [CommonModule ,TableModule, FormsModule, DialogModule, ButtonModule, ToastModule],
  templateUrl: './personal-addresses.component.html',
  styleUrl: './personal-addresses.component.css',
  providers: [MessageService]
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


  constructor(private httpAddresses: HttpUserAdminService, private messageService: MessageService, private primengConfig: PrimeNGConfig) {}

  ngOnInit(): void {
    this.getUserAddresses();
    this.primengConfig.ripple = true;
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

  showSuccess(content: string) {
    this.messageService.add({severity:'success', summary: 'Success', detail: content});
  }

  showError(content: string) {
    this.messageService.add({severity:'error', summary: 'Error', detail: content});
  }

  SubmitAddress(newForm: NgForm){
    this.newAddress = newForm.value as AddressFormData;
    this.httpAddresses.httpPostCustomerAddress(this.newAddress).subscribe({
      next: (response: any) => {
        if(HttpStatusCode.Ok)
          this.showSuccess('Indirizzo aggiunto con successo')
      },
      error: (err: Error) => {
        this.showError("Errore nell'aggiunta")
        console.log(err)
      },
    });
  }

  PutModifyAddress(){
    //DA COMPLETARE
    this.newAddress = new AddressFormData(this.modifyAddress.addressLine1, this.modifyAddress.addressLine2,
                     this.modifyAddress.city, this.modifyAddress.stateProvince, this.modifyAddress.countryRegion,
                    this.modifyAddress.postalCode, this.typeAddress);
                    console.log(this.newAddress);
    this.httpAddresses.httpPutCustomerAddress(this.newAddress, this.modifyAddress.addressId).subscribe({
      next: (response: any) => {
        if(HttpStatusCode.Ok)
          this.showSuccess('Modifica avvenuta con successo')
      },
      error: (err: Error) => {
        this.showError('Errore nella modifica')
        console.log(err)
      },
    });
  }

  DeleteAddress(id: number){
    this.httpAddresses.httpDeleteCustomerAddress(id).subscribe({
      next: (response: any) => {
        if(HttpStatusCode.Ok)
          this.showSuccess('Eliminazione avvenuta con successo')
      },
      error: (err: Error) => {
        this.showError('Errore nella cancellazione')
        console.log(err)
      },
    });
  }
}
