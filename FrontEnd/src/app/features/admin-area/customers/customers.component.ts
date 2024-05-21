import { Component, OnInit } from '@angular/core';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Customer } from '../../../shared/models/Customer';
import { PaginatorModule } from 'primeng/paginator';
import { SearchParams } from '../../../shared/models/SearchParams';
import { TableModule } from 'primeng/table';
import { FilterService } from 'primeng/api';
@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [FormsModule, CommonModule, PaginatorModule, TableModule],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css',
})
export class CustomersComponent {
  //ArrayCheSiMostra
  customers!: Customer[];

  searchParams: SearchParams = new SearchParams();
  first: number = 0;
  rows: number = 10;
  customersCount: number = 0;
  searchName: string = '';

  constructor(private AdminService: HttpUserAdminService) {}
  ngOnInit() {
    this.funzioneCustomer();
  }
  funzioneCustomer() {
    this.AdminService.searchParams$.subscribe(
      (par) => (this.searchParams = par)
    );
    this.searchParams.pageIndex = 1;
    this.searchParams.search = this.searchName;
    this.searchParams.pageSize = 10;
    this.AdminService.httpGetCustomerInfo(this.searchParams).subscribe({
      next: (customers: any) => {
        console.log(customers);
        this.customers = customers.item2;
        this.customersCount = customers.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }

  changeOutput(event: any) {
    this.searchParams.pageIndex = event.page! + 1;
    this.searchParams.pageSize = event.rows!;
    this.searchParams.search = this.searchName;
    this.AdminService.searchParams$.subscribe(
      (par) => (this.searchParams = par)
    );
    this.AdminService.httpGetCustomerInfo(this.searchParams).subscribe({
      next: (customer: any) => {
        console.log(customer);
        this.customers = customer.item2;
        this.customersCount = customer.item1;
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
}
