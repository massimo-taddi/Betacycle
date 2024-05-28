import { Routes } from '@angular/router';

import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './core/login/login.component';
import { SignUpComponent } from './core/sign-up/sign-up.component';
import { SearchComponent } from './features/search/search.component';
import { PrivateAreaComponent } from './features/private-area/private-area.component';
import { PersonalOrdersComponent } from './features/private-area/personal-orders/personal-orders.component';
import { PersonalAddressesComponent } from './features/private-area/personal-addresses/personal-addresses.component';
import { PersonalInfoComponent } from './features/private-area/personal-info/personal-info.component';
import { AdminAreaComponent } from './features/admin-area/admin-area.component';
import { CustomersComponent } from './features/admin-area/customers/customers.component';
import { AddProductComponent } from './features/admin-area/add-product/add-product.component';
import { ProductsListComponent } from './features/admin-area/products-list/products-list.component';
import { ModifyProductComponent } from './features/admin-area/modify-product/modify-product.component';
import { PasswordResetComponent } from './core/password-reset/password-reset.component';
import { PasswordForgotComponent } from './core/password-reset/password-forgot/password-forgot.component';
import { PasswordForgotResetComponent } from './core/password-reset/password-forgot/password-forgot-reset/password-forgot-reset.component';

import { CategoryComponent } from './features/admin-area/category/category.component';
import { ModelComponent } from './features/admin-area/model/model.component';
import { BasketComponent } from './core/basket/basket.component';
import { ProductPageComponent } from './features/product-page/product-page.component';
import { ModifyCategoryComponent } from './features/admin-area/modify-category/modify-category.component';
import { ModifyModelComponent } from './features/admin-area/modify-model/modify-model.component';
import { AddCategoryComponent } from './features/admin-area/add-category/add-category.component';
import { AddModelComponent } from './features/admin-area/add-model/add-model.component';
import { CheckoutComponent } from './core/checkout/checkout.component';
import { OrderSummaryComponent } from './core/checkout/order-summary/order-summary.component';
import { SiteReviewComponent } from './features/site-review/site-review.component';

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
    path: 'search/:search/:pageIndex/:pageSize/:sort',
    component: SearchComponent,
  },
  {
    path: 'basket',
    component: BasketComponent,
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
  },
  {
    path: 'order-summary',
    component: OrderSummaryComponent,
  },
  {
    path: 'product-page/:productId',
    component: ProductPageComponent,
  },
  {
    path: 'personal',
    component: PrivateAreaComponent,
    children: [
      {
        path: 'myorders',
        component: PersonalOrdersComponent,
      },
      {
        path: 'myaddresses',
        component: PersonalAddressesComponent,
      },
      {
        path: 'myinfo',
        component: PersonalInfoComponent,
        children: [
          {
            path: 'pwreset',
            component: PasswordResetComponent,
          },
        ],
      },
    ],
  },

  {
    path: 'forgotpwd',
    component: PasswordForgotComponent,
  },
  {
    path: 'resetforgot',
    component: PasswordForgotResetComponent,
  },
  {
    path: 'sitereview',
    component: SiteReviewComponent,
  },

  {
    path: 'admin',
    component: AdminAreaComponent,
    children: [
      {
        path: 'category',
        component: CategoryComponent,
      },
      {
        path: 'model',
        component: ModelComponent,
      },
      {
        path: 'add-product',
        component: AddProductComponent,
      },
      {
        path: 'customers',
        component: CustomersComponent,
      },
      {
        path: 'products-list',
        component: ProductsListComponent,
      },
      {
        path: 'modify-product',
        component: ModifyProductComponent,
      },
      {
        path: 'modify-category',
        component: ModifyCategoryComponent,
      },
      {
        path: 'modify-model',
        component: ModifyModelComponent,
      },
      {
        path: 'add-category',
        component: AddCategoryComponent,
      },
      {
        path: 'add-model',
        component: AddModelComponent,
      },
    ],
  },
];
