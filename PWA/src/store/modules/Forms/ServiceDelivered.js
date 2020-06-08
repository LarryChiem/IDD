import Vue from "vue";
import { getField, updateField } from "vuex-map-fields";

const mutations = {
  updateField,
  incrementEdited(state, count) {
    state.totalEdited += count;
  },
  updateTimesheet(state, value) {
    // Create a deep copy of timesheet for any change
    // Inefficient, but works
    Vue.set(
      state.formFields.timesheet,
      "value",
      JSON.parse(JSON.stringify(value))
    );
  },
  resetState(state) {
    // acquire initial state
    const s = initialState();
    state["totalEdited"] = s["totalEdited"];
    state["willResign"] = s["willResign"];
    Object.keys(s["formFields"]).forEach((key) => {
      Vue.set(
        state["formFields"],
        key,
        JSON.parse(JSON.stringify(s["formFields"][key]))
      );
    });
  },
};

const getters = {
  getField,
};

const initialState = () => ({
  formFields: {
    clientName: { value: null, parsed_value: null, disabled: false },
    prime: { value: null, parsed_value: null, disabled: false },
    submissionDate: { value: null, parsed_value: null, disabled: false },
    providerName: { value: null, parsed_value: null, disabled: false },
    providerNum: { value: null, parsed_value: null, disabled: false },
    scpaName: { value: null, parsed_value: null, disabled: false },
    brokerage: { value: null, parsed_value: null, disabled: false },
    serviceAuthorized: { value: null, parsed_value: null, disabled: false },
    units: { value: null, parsed_value: null, disabled: false },
    type: { value: null, parsed_value: null, disabled: false },
    frequency: { value: null, parsed_value: null, disabled: false },
    timesheet: { value: null, parsed_value: null, disabled: false },
    totalHours: { value: null, parsed_value: null, disabled: false },
    serviceGoal: { value: null, parsed_value: null, disabled: false },
    progressNotes: { value: null, parsed_value: null, disabled: false },
    employerSignature: { value: null, parsed_value: null, disabled: false },
    employerSignDate: { value: null, parsed_value: null, disabled: false },
    providerSignature: { value: null, parsed_value: null, disabled: false },
    providerSignDate: { value: null, parsed_value: null, disabled: false },
    authorization: { value: null, parsed_value: null, disabled: false },
    approval: { value: null, parsed_value: null, disabled: false },
  },
  totalEdited: 0,
  willResign: false,
});

export default {
  namespaced: true,
  mutations,
  getters,
  state: initialState(),
};
