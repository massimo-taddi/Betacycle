import { Component } from '@angular/core';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { InputNumberModule } from 'primeng/inputnumber';
import { TabViewModule } from 'primeng/tabview';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { SidebarModule } from 'primeng/sidebar';
@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    BreadcrumbModule,
    InputNumberModule,
    TabViewModule,
    CardModule,
    ButtonModule,
    SidebarModule,
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css',
})
export class SearchComponent {}
