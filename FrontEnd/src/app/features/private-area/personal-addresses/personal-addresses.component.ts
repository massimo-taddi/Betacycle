import { Component, OnInit } from '@angular/core';
import { Address } from '../../../shared/models/Address';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';

@Component({
  selector: 'app-personal-addresses',
  standalone: true,
  imports: [CommonModule ,TableModule, DialogModule],
  templateUrl: './personal-addresses.component.html',
  styleUrl: './personal-addresses.component.css'
})
export class PersonalAddressesComponent implements OnInit {
  addresses: Address[] = [];

  constructor(private httpAddresses: HttpUserAdminService) {}

  ngOnInit(): void {
    this.getUserAddresses();
  }

  private getUserAddresses() {
    this.httpAddresses.httpGetCustomerAddresses().subscribe({
      next: (addressList: Address[]) => {
        this.addresses = addressList
        console.log(this.addresses)
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
}
