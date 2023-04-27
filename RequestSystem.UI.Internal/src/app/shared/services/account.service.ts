import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Token } from '../../models/common';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  apiURL = environment.apiURL;
  loginUrl = this.apiURL + '/Account/login';

  tokenResp: any;
  token!: Token;

  constructor(private http: HttpClient, private router: Router) { }

  login() {
    const body = {
      "userName": "abc",
      "password": "abc1"
    };

    this.http.post(this.apiURL + '/Account/login', body).subscribe(result => {

      if (result != null) {
        this.token = result as Token;

        sessionStorage.setItem('token', this.token.token);
        sessionStorage.setItem('username', this.token.userName);
      } else {
        alert('Login failed');
      }
    }, error => console.error(error)); 
  }

  isLoggedIn() {
    return sessionStorage.getItem("token") != null;
  }

  getToken() {
    return sessionStorage.getItem("token") || '';
  }

  logout() {
    alert("your session is expired");
    sessionStorage.clear();
    //this.router.navigateByUrl("/login");
  }

}
