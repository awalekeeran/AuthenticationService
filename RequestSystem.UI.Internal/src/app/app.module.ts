import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { AccordionComponent } from './shared/accordion.component';
import { HeaderInterceptor } from './shared/interceptors/header.interceptor';
import { LoggingInterceptor } from './shared/interceptors/logging.interceptor';
import { AccountService } from './shared/services/account.service';

@NgModule({
  declarations: [
    AppComponent,
    AccordionComponent
  ],
  imports: [
    BrowserModule, HttpClientModule
  ],
  providers: [
    AccountService,
    { provide: HTTP_INTERCEPTORS, useClass: LoggingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: HeaderInterceptor, multi: true }
   
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
