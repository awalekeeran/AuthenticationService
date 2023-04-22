const PROXY_CONFIG = [
  {
    context: [
      "/api/weatherforecast",
    ],
    target: "http://localhost:5267",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
