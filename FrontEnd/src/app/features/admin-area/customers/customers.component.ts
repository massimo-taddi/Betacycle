import { Component, OnInit } from '@angular/core';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Customer } from '../../../shared/models/Customer';
import { PaginatorModule } from 'primeng/paginator';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [FormsModule, CommonModule, PaginatorModule],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css',
})
export class CustomersComponent {
  customers!: Customer[];
  searchParams: SearchCustomer = new SearchCustomer();
  first: number = 0;
  rows: number = 10;
  customersCount: number = 0;

  constructor(private AdminService: HttpUserAdminService) {}
  ngOnInit() {
    this.funzioneCustomer(1);
  }
  funzioneCustomer(index: number) {
    this.searchParams.pageIndex = index;
    this.searchParams.pageSize = 10;
    this.AdminService.searchParams$.subscribe(
      (par) => (this.searchParams = par)
    );
    this.AdminService.httpGetCustomerInfo(this.searchParams).subscribe({
      next: (customers: any) => {
        this.customers = customers.item2;
        this.customersCount = customers.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
}
