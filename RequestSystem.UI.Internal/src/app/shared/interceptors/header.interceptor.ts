import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, throwError, Observable } from 'rxjs';
import { AccountService } from '../services/account.service';

@Injectable()
export class HeaderInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const API_KEY = '123456';
    const Authorization = "Bearer " + this.accountService.getToken();
    return next.handle(request.clone(
      {
        setHeaders: {
          API_KEY,
          Authorization
        }
      }
    )).pipe(
      catchError(errordata => {
        if (errordata.status === 401) {
          // need to implement logout
          this.accountService.logout();

          // refresh token logic
        }
        return throwError(errordata);
      })
    );
  }
}
