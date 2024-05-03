import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { InputGroupModule } from 'primeng/inputgroup';
import { FormsModule } from '@angular/forms';
import { Product } from '../../shared/models/Product';
@Component({
  selector: 'app-admin-area',
  standalone: true,
  imports: [
    ButtonModule,
    RouterModule,
    CommonModule,
    InputGroupModule,
    FormsModule,
  ],
  templateUrl: './admin-area.component.html',
  styleUrl: './admin-area.component.css',
})
export class AdminAreaComponent {
  arrayProdotti: Product[] = [];
  Delete() {
    console.log('Cancellato');
  }
  Update() {
    console.log('aggiornato');
  }
  Read() {
    console.log('leggi');
  }
  onInit() {
    return this.arrayProdotti;
  }
}
