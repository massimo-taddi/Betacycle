import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { SalesOrderHeader } from '../../../shared/models/SalesOrderHeader';
import { HttpOrdersService } from '../../../shared/services/http-orders.service';

@Component({
  selector: 'app-personal-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './personal-orders.component.html',
  styleUrl: './personal-orders.component.css'
})
export class PersonalOrdersComponent implements OnInit {
  orders: SalesOrderHeader[] = [];
  constructor(private httpOrders: HttpOrdersService) {}
  ngOnInit(): void {
    this.getPersonalItems();
  }
  private getPersonalItems() {
    this.httpOrders.httpGetUserOrders().subscribe({
      next: (orders: any) => {
        orders.array.forEach((element: SalesOrderHeader) => {
          this.orders.push(element)
        });
      },
      error: (err: any) => {
        console.log(err);
      },
    });
  }
}
