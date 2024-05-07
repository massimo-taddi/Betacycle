import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { SalesOrderHeader } from '../../../shared/models/SalesOrderHeader';
import { HttpUserAdminService } from '../../../shared/services/http-user-admin.service';
import { TableModule } from 'primeng/table';
import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { Product } from '../../../shared/models/Product';
import { ProductService } from '../../../shared/services/product.service';


@Component({
  selector: 'app-personal-orders',
  standalone: true,
  imports: [CommonModule, TableModule, BadgeModule, ButtonModule, DialogModule],
  templateUrl: './personal-orders.component.html',
  styleUrl: './personal-orders.component.css'
})
export class PersonalOrdersComponent implements OnInit {
  orders: SalesOrderHeader[] = [];
  myProducts: Product[] = [];
  dialogBool: boolean = false;
  constructor(private httpOrders: HttpUserAdminService, private httpProducts: ProductService) { }
  ngOnInit(): void {
    this.getPersonalItems();
  }

  /// Order current status. 1 = In process; 2 = Approved; 3 = Backordered; 4 = Rejected; 5 = Shipped; 6 = Cancelled
  orderStatusString(status: number): string {
    let box = document.getElementById('status-box');
    switch (status) {
      case 1:
        box?.setAttribute('style','background-color: rgb(240, 204, 152); color: darkorange; padding: 5px; border-radius: 10px;');
        return 'In process';
      case 2:
        box?.setAttribute('style','background-color: lightgreen; color: darkgreen; padding: 5px; border-radius: 10px;');
        return 'Approved';
      case 3:
        box?.setAttribute('style','background-color: rgb(240, 204, 152); color: darkorange; padding: 5px; border-radius: 10px;');
        return 'Backordered';
      case 4:
        box?.setAttribute('style','background-color: rgb(249, 142, 142); color: darkred; padding: 5px; border-radius: 10px;');
        return 'Rejected';
      case 5:
        box?.setAttribute('style','background-color: lightgreen; color: darkgreen; padding: 5px; border-radius: 10px;');
        return 'Shipped';
      case 6:
        box?.setAttribute('style','background-color: rgb(249, 142, 142); color: darkred; padding: 5px; border-radius: 10px;');
        return 'Cancelled';
    }
    return 'ERRORE';
  }

  private getPersonalItems() {
    this.httpOrders.httpGetUserOrders().subscribe({
      next: (orders: SalesOrderHeader[]) => {
        for (let order of orders) {
          console.log(order)
          this.orders.push(order);
        }
      },
      error: (err: Error) => {
        console.log('SONO QUI');
      },
    });
  }

  getOrderProducts(ord: SalesOrderHeader) {
    this.myProducts= [];
    ord.salesOrderDetails.forEach(det => {
      this.httpProducts.getProductById(det.productId).subscribe({
        next: (product: Product) => {
          this.myProducts.push(product);
        },
        error: (err: Error) => {
          console.log(err.message);
        },
      });
    })
    console.log(this.myProducts);
  };

}
