import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TagModule } from 'primeng/tag';
import { CarouselModule } from 'primeng/carousel';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TagModule,
    CarouselModule,
    ButtonModule,
    RouterModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  images: any[] = [
    '../../../assets/images/BiciclettaProva1.png',
    '../../../assets/images/BiciclettaProva2.jpg',
    '../../../assets/images/BiciclettaProva3.jpg',
  ];
}
