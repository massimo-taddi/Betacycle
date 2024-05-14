import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { BasketItem } from '../../shared/models/BasketItem';

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './basket.component.html',
  styleUrl: './basket.component.css'
})
export class BasketComponent {
  basketItems: BasketItem[] = [];

  calculateTotalPrice() { }
}
