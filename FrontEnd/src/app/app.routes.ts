import { Routes } from '@angular/router';

import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './core/login/login.component';
import { SignUpComponent } from './core/sign-up/sign-up.component';
import { SearchComponent } from './features/search/search.component';
import { PrivateAreaComponent } from './features/private-area/private-area.component';
import { PersonalOrdersComponent } from './features/private-area/personal-orders/personal-orders.component';
import { PersonalAddressesComponent } from './features/private-area/personal-addresses/personal-addresses.component';
import { PersonalPaymentsComponent } from './features/private-area/personal-payments/personal-payments.component';
import { PersonalInfoComponent } from './features/private-area/personal-info/personal-info.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'signup',
    component: SignUpComponent,
  },
  {
    path: 'search',
    component: SearchComponent,
  },
  {
    path: 'personal',
    component: PrivateAreaComponent,
    children: [
      {
        path: 'myorders',
        component: PersonalOrdersComponent
      },
      {
        path: 'myaddresses',
        component: PersonalAddressesComponent
      },
      {
        path: 'mypayments',
        component: PersonalPaymentsComponent
      },
      {
        path: 'myinfo',
        component: PersonalInfoComponent
      },
    ]
  },
];
