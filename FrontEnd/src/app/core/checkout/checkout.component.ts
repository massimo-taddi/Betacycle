import { Component, OnInit } from '@angular/core';
import { HttpUserAdminService } from '../../shared/services/http-user-admin.service';
import { Address } from '../../shared/models/Address';
import { CommonModule } from '@angular/common';
import { SalesOrderHeader } from '../../shared/models/SalesOrderHeader';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit{
  addresses: Address []=[];
  salesOrderHeader: SalesOrderHeader = new SalesOrderHeader();

  constructor(private http: HttpUserAdminService){}

  ngOnInit(): void {
    this.http.httpGetCustomerAddresses().subscribe({
      next: (data: Address[]) =>{this.addresses = data; console.log(this.addresses)},
      error: (err: Error) => {console.log(err)}
    })
  }

  setShipAddress(name: string){
    const radios = document.querySelectorAll<HTMLInputElement>(`input[name="${name}"]:checked`);
    this.salesOrderHeader.shipToAddressID = this.addresses[radios[0].value as unknown as number].addressId;
  }
  setBillAddress(name: string){
    const radios = document.querySelectorAll<HTMLInputElement>(`input[name="${name}"]:checked`);
    this.salesOrderHeader.billToAddressID = this.addresses[radios[0].value as unknown as number].addressId;
  }
}
