import Vue from "vue";
import Vuex from "vuex";
import VuexPersistence from 'vuex-persist'
import { getField, updateField } from "vuex-map-fields";

// Import the other vuex stores, for specific form components
import Mileage from "@/store/modules/Forms/Mileage";
import ServiceDelivered from "@/store/modules/Forms/ServiceDelivered";

Vue.use(Vuex);

const debug = process.env.NODE_ENV !== "production";

// Initialization state / reset state for the cached vuex store
const initialState = () => ({
  formChoice: null,
  formId: 0,
  onlineStatus: true,
  newForm: true,
  invalidForm: false,
});

// Setup for saving the cached vuex store to localStorage
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
      // Hold onto the initial/reset state
      const s = initialState();
      const online = state["onlineStatus"];

      // Reset the current vuex store state to the initial/reset state
      // Doing it this way will not break the reactivity of the vuex store
      Object.keys(s).forEach((key) => {
        state[key] = s[key];
      });
      state["onlineStatus"] = online;
    },
  },
  strict: debug,
  plugins: [vuexLocal.plugin],
});
