import Vue from "vue";
import Vuex from "vuex";
import { getField, updateField } from "vuex-map-fields";
import VuexPersistence from "vuex-persist";

import Mileage from "@/store/modules/Forms/Mileage";
import ServiceDelivered from "@/store/modules/Forms/ServiceDelivered";

Vue.use(Vuex);

const debug = process.env.NODE_ENV !== "production";
const initialState = () => ({
  formChoice: null,
  formId: 0,
  onlineStatus: true,
  newForm: true,
  invalidForm: false,
});

const vuexLocal = new VuexPersistence({
  storage: window.localStorage,
});

export default new Vuex.Store({
  modules: {
    Mileage,
    ServiceDelivered,
  },
  state: initialState,
  getters: {
    getField,
  },
  mutations: {
    updateField,
    resetState(state) {
      // acquire initial state
      const s = initialState();
      const online = state["onlineStatus"];
      Object.keys(s).forEach((key) => {
        state[key] = s[key];
      });
      state["onlineStatus"] = online;
    },
  },
  strict: debug,
  plugins: [vuexLocal.plugin],
});
