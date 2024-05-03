import { Component, OnInit } from '@angular/core';
import { InputGroupModule } from 'primeng/inputgroup';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../shared/models/Product';
@Component({
  selector: 'app-aggiungi',
  standalone: true,
  imports: [InputGroupModule, CommonModule, FormsModule],
  templateUrl: './aggiungi.component.html',
  styleUrl: './aggiungi.component.css',
})
export class AggiungiComponent {
  arrayProdotti: Product[] = [];
  ngOnInit() {}
}
