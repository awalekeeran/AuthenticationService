const PROXY_CONFIG = [
  {
    context: [
      "weatherforecast"
    ],
    target: "http://localhost:5267",
    secure: false,
    changeOrigin: false,
    pathRewrite: {
      "^/api/weatherforecast": "/api/v1/WeatherForecast"
    },
  }
]

module.exports = PROXY_CONFIG;
