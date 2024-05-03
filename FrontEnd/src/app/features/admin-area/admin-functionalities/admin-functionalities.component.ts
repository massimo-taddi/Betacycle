import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
@Component({
  selector: 'app-admin-functionalities',
  standalone: true,
  imports: [ButtonModule, RouterModule],
  templateUrl: './admin-functionalities.component.html',
  styleUrl: './admin-functionalities.component.css',
})
export class AdminFunctionalitiesComponent {
  //questo ts servirà per funzionalità come modifica elimina etc
}
