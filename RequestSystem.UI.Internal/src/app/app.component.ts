import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { environment } from './../environments/environment';
import { AccountService } from './shared/services/account.service';
import { WeatherForecast, City, Token } from './models/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = environment.title;
  apiURL = environment.apiURL;

  forecasts?: WeatherForecast[];
  cities?: City[];

  isHidden = false;
  imageUrl: any;
  base64String: string | undefined;  
  sessionStorage!: Token ;

  httpClient!: HttpClient;

  constructor(http: HttpClient, private sanitizer: DomSanitizer, private accountService: AccountService) {
    this.httpClient = http;

    this.AuthenticateUser();
    this.GetWeatherForecast();
    this.GetAllCity();
  }
  AuthenticateUser() {
    this.accountService.login();
    }

  private GetWeatherForecast() {
    this.httpClient.get<WeatherForecast[]>(this.apiURL + '/weatherforecast').subscribe(result => {
            this.forecasts = result;
        }, error => console.error(error));
  }

  private GetAllCity() {
    this.httpClient.get<City[]>(this.apiURL + '/City').subscribe(result => {
      this.cities = result;
      this.cities.forEach(city => {
        city.cityImage = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${city.cityImage}`)
      });
    }, error => console.error(error));
  }

  private ConvertToImage() {
    this.imageUrl = this.base64String;
  }
  private IsValidString() {
    return this.base64String && this.base64String.length > 0;
  }

  private IsValidImage() {
    return this.imageUrl && this.imageUrl.length > 0;
  }
}
