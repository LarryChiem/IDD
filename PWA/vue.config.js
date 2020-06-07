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
          "src": "assets/icons/logo_short-192x192.png",
          "sizes": "192x192"
        },
        {
          "src": "assets/icons/logo_short-512x512.png",
          "sizes": "512x512"
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
