import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { environment } from './../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = environment.title;
  apiURL = environment.apiURL;

  public forecasts?: WeatherForecast[];
  public cities?: City[];

  isHidden = false;
  imageUrl: any;
  base64String: string | undefined;  

  constructor(http: HttpClient, private sanitizer: DomSanitizer) {
    this.GetWeatherForecast(http);
    this.GetAllCity(http);
  }

  private GetWeatherForecast(http: HttpClient) {
    http.get<WeatherForecast[]>(this.apiURL + '/weatherforecast').subscribe(result => {
            this.forecasts = result;
        }, error => console.error(error));
  }

  private GetAllCity(http: HttpClient) {
    http.get<City[]>(this.apiURL + '/City').subscribe(result => {
      this.cities = result;
      this.cities.forEach(city => {
        city.cityImage = this.sanitizer.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${city.cityImage}`)
      });
      console.log(result);
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

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface City {
  id: number;
  name: string;
  imageUrl: string;
  cityImage: any;
}
