import App from "./App.vue";
import FlagIcon from 'vue-flag-icon';
import i18n from '@/plugins/i18n';
import router from "./router";
import Vue from "vue";
import vuetify from "@/plugins/vuetify";
import store from "./store";
import "./registerServiceWorker";

// Load flag icons to indicate the selected display language
Vue.use(FlagIcon);

Vue.config.productionTip = false;
Vue.config.devtools = true;
Vue.filter("formatSize", function (size) {
  if (size > 1024 * 1024 * 1024 * 1024) {
    return (size / 1024 / 1024 / 1024 / 1024).toFixed(2) + " TB";
  } else if (size > 1024 * 1024 * 1024) {
    return (size / 1024 / 1024 / 1024).toFixed(2) + " GB";
  } else if (size > 1024 * 1024) {
    return (size / 1024 / 1024).toFixed(2) + " MB";
  } else if (size > 1024) {
    return (size / 1024).toFixed(2) + " KB";
  }
  return size.toString() + " B";
});

// Mount a new Vue instance onto the page
new Vue({
  i18n,
  router,
  store,
  vuetify,
  render: (h) => h(App),
}).$mount("#app");
