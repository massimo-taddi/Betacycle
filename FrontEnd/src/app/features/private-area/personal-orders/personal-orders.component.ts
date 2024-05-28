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
import { SalesOrderDetail } from '../../../shared/models/SalesOrderDetail';
import { last, lastValueFrom } from 'rxjs';


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
  dialogBools: boolean[] = [];

  constructor(private httpOrders: HttpUserAdminService, private httpProducts: ProductService) { }

  ngOnInit(): void {
    this.getPersonalItems();
    for(var i=0; i< this.orders.length;i++)
      this.dialogBools.push(false);
  }

  /// Order current status. 1 = In process; 2 = Approved; 3 = Backordered; 4 = Rejected; 5 = Shipped; 6 = Cancelled
  orderStatusString(status: number){
    let box = Array.from(document.getElementsByClassName('status-box'));
    box.forEach(el => {
      switch (status) {
        case 1:
          el?.setAttribute('style','background-color: rgb(240, 204, 152); color: darkorange; padding: 5px; border-radius: 10px;');
          el.innerHTML = 'In process';
          break;
        case 2:
          el?.setAttribute('style','background-color: lightgreen; color: darkgreen; padding: 5px; border-radius: 10px;');
          el.innerHTML = 'Approved';
          break;
        case 3:
          el?.setAttribute('style','background-color: rgb(240, 204, 152); color: darkorange; padding: 5px; border-radius: 10px;');
          el.innerHTML = 'Backordered';
          break;
        case 4:
          el?.setAttribute('style','background-color: rgb(249, 142, 142); color: darkred; padding: 5px; border-radius: 10px;');
          el.innerHTML = 'Rejected';
          break;
        case 5:
          el?.setAttribute('style','background-color: lightgreen; color: darkgreen; padding: 5px; border-radius: 10px;');
          el.innerHTML = 'Shipped';
          break;
        case 6:
          el?.setAttribute('style','background-color: rgb(249, 142, 142); color: darkred; padding: 5px; border-radius: 10px;');
          el.innerHTML = 'Cancelled';
          break;
        default:
          el.innerHTML = 'ERRORE'
          break;
      }
    })
  }

  private async getPersonalItems() { //ottengo salesorderheaders con soli id senza oggetti annessi
    this.orders = await lastValueFrom(this.httpOrders.httpGetUserOrders())
    this.fillAddresses();   
  }

  private async fillAddresses(){
    this.orders.forEach(async ord => {
      ord.shipToAddress = await lastValueFrom(this.httpOrders.httpGetSingleAddress(ord.shipToAddressId))
      console.log(ord.shipToAddress)
    });
  }

  public async getDetails(headerId: number){
    var myOrder: SalesOrderHeader| undefined = this.orders.find(ord=> ord.salesOrderId == headerId)
    myOrder!.salesOrderDetails = (await lastValueFrom(this.httpOrders.httpGetDetailsFromHeader(headerId))) as SalesOrderDetail[];
    console.log(this.orders)
  }

}
