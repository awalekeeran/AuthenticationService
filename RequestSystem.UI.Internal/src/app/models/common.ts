export class WeatherForecast {
  date!: string;
  temperatureC!: number;
  temperatureF!: number;
  summary!: string;
}

export class City {
  id!: number;
  name!: string;
  imageUrl!: string;
  cityImage!: any;
}

export class Token {
  expires!: string;
  userName!: string;
  token!: string;
  refreshToken!: string;
}
