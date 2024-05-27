import { Component, OnInit } from '@angular/core';
import { CheckoutService } from '../../../shared/services/checkout.service';
import { SalesOrderHeader } from '../../../shared/models/SalesOrderHeader';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-order-summary',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.css',
  providers: []
})
export class OrderSummaryComponent implements OnInit {
  postedOrder: SalesOrderHeader = new SalesOrderHeader();
  
  constructor(private checkoutService: CheckoutService) { }

  ngOnInit(): void {
    this.checkoutService.postResultOrderHeader$.subscribe({
      next: (data: SalesOrderHeader) => {
        this.postedOrder = data;
      },
      error: (err: Error) => {
        console.log(err.message);
      }
    });
  }
}
