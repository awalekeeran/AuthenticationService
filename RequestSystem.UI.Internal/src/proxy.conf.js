const PROXY_CONFIG = [
  {
    context: [
      "weatherforecast",
    ],
    target: "http://localhost:5267",
    secure: false,
    changeOrigin:true
  }
]

module.exports = PROXY_CONFIG;
