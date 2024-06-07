import { Component, OnInit } from '@angular/core';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Customer } from '../../../shared/models/Customer';
import { PaginatorModule } from 'primeng/paginator';
import { SearchParams } from '../../../shared/models/SearchParams';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    PaginatorModule,
    TableModule,
    ButtonModule,
    ToastModule
  ],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css',
  providers: [MessageService]
})
export class CustomersComponent {
  customers!: Customer[];

  searchParams: SearchParams = new SearchParams();
  first: number = 0;
  rows: number = 10;
  customersCount: number = 0;
  searchName: string = '';

  constructor(private AdminService: HttpUserAdminService, private messageService: MessageService) {}
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
  DeleteCustomer(id: number) {
    this.AdminService.httpDeleteCustomerAdmin(id).subscribe({
      next: (el: any) => {
        console.log(el);
        this.deleteSuccess();
        this.ngOnInit();
        // window.location.reload();
      },
      error: (err: any) => {
        console.log(err);
      },
    });
  }

  deleteSuccess() {
    this.messageService.add({
      severity: 'success',
      summary: 'Success',
      detail: 'Customer deleted',
    });
  }
}
