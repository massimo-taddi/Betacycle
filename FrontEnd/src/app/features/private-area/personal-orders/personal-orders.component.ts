import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { SalesOrderHeader } from '../../../shared/models/SalesOrderHeader';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { TableModule } from 'primeng/table';
import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';


@Component({
  selector: 'app-personal-orders',
  standalone: true,
  imports: [CommonModule, TableModule, BadgeModule, ButtonModule, DialogModule],
  templateUrl: './personal-orders.component.html',
  styleUrl: './personal-orders.component.css'
})
export class PersonalOrdersComponent implements OnInit {
  orders: SalesOrderHeader[] = [];
  dialogBool: boolean = false;
  constructor(private httpOrders: HttpUserAdminService) {}
  ngOnInit(): void {
    this.getPersonalItems();
  }

  /// Order current status. 1 = In process; 2 = Approved; 3 = Backordered; 4 = Rejected; 5 = Shipped; 6 = Cancelled
  orderStatusString(status: number): string {
    switch(status) {
      case 1:
        return 'In process';
      case 2:
        return 'Approved';
      case 3:
        return 'Backordered';
      case 4:
        return 'Rejected';
      case 5:
        return 'Shipped';
      case 6:
        return 'Cancelled';
    }
    return 'ERRORE';
  }

  private getPersonalItems() {
    this.httpOrders.httpGetUserOrders().subscribe({
      next: (orders: SalesOrderHeader[]) => {
        for(let order of orders) {
          this.orders.push(order);
        }
      },
      error: (err: Error) => {
        console.log(err.message);
      },
    });
  }
  
}
