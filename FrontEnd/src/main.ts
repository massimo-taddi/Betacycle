import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { RouterModule } from '@angular/router';
import { routes } from './app/app.routes';
import { importProvidersFrom } from '@angular/core';

appConfig.providers.push(importProvidersFrom(RouterModule.forRoot(routes, {scrollPositionRestoration: 'enabled'})));

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));