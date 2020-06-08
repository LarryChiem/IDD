module.exports = {
  "pwa": {
    "name": "IDD Timesheet Submission Application",
    "short_name": "IDD Timesheets",
    "theme_color": "green",
    "background_color": "white",
    "display": "standalone",
    "start_url": ".",
    "iconPaths": {
      "favicon16": "img/icons/favicon-16x16.png",
      "favicon32": "img/icons/favicon-32x32.png",
      "icons": [
        {
          "src": "img/icons/logo_short-72x72.png",
          "sizes": "72x72",
          "type": "image/png"
        },
        {
          "src": "img/icons/logo_short-96x96.png",
          "sizes": "96x96",
          "type": "image/png"
        },
        {
          "src": "img/icons/logo_short-128x128.png",
          "sizes": "128x128",
          "type": "image/png"
        },
        {
          "src": "img/icons/logo_short-144x144.png",
          "sizes": "144x144",
          "type": "image/png"
        },
        {
          "src": "img/icons/logo_short-152x152.png",
          "sizes": "152x152",
          "type": "image/png"
        },
        {
          "src": "img/icons/logo_short-192x192.png",
          "sizes": "192x192",
          "type": "image/png"
        },
        {
          "src": "img/icons/logo_short-384x384.png",
          "sizes": "384x384",
          "type": "image/png"
        },
        {
          "src": "img/icons/logo_short-512x512.png",
          "sizes": "512x512",
          "type": "image/png"
        }
      ]
    },
    "manifestPath": "manifest.json",
    workboxOptions: {
      navigateFallback: 'index.html'
    }
  },
  "transpileDependencies": [
    "vuetify"
  ]
}
