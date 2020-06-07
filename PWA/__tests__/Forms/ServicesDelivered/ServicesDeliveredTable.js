import Vue from "vue";
import Vuetify from "vuetify";
import store from "@/store/index.js";
import i18n from '@/plugins/i18n';
import ServicesDeliveredTable from "@/components/Forms/ServicesDelivered/ServicesDeliveredTable.vue";
import valid_timesheet from "./valid_timesheet.json";
import conflicting_timesheet from "./conflicting_timesheet.json";

import { mount, createLocalVue } from "@vue/test-utils";

Vue.use(Vuetify, i18n);

const valCols = ["date", "starttime", "endtime", "totalHours"];
let amtErrors = 0;

describe("ServicesDeliveredTable.js", () => {
  let valid_props = {};
  valid_props["parsed_value"] = valid_timesheet["timesheet"];
  valid_props["value"] = valid_timesheet["timesheet"];
  valid_props["disabled"] = true;
  valid_props["cols"] = valCols;

  // Given no values, the table should still load
  it("Given empty props, the table should load with no entries or errors", () => {
    const localVue = createLocalVue();
    let wrapper = mount(ServicesDeliveredTable, {
      localVue,
      i18n,
      store,
      vuetify: new Vuetify(),
      propsData: { cols: valCols },
    });
    expect(wrapper.vm.allEntries.length).toBe(0);
    expect(wrapper.vm.amtEdited).toBe(0);
    expect(wrapper.vm.validate()).toBe(0);
    wrapper.destroy();
  });

  // Given a valid timesheet table .json, the user should not get an error.
  it("Given valid props, the table should load with no errors", () => {
    const localVue = createLocalVue();
    let wrapper = mount(ServicesDeliveredTable, {
      localVue,
      i18n,
      store,
      vuetify: new Vuetify(),
      propsData: valid_props,
    });
    expect(wrapper.vm.allEntries.length).toBe(valid_props["value"].length);
    expect(wrapper.vm.amtEdited).toBe(0);
    amtErrors = wrapper.vm.validate();
    wrapper.vm.printErrors();
    expect(amtErrors).toBe(0);
    wrapper.destroy();
  });
});

describe("ServicesDeliveredTable.js", () => {
  // Invalid timesheet entries should have an error
  it("Given mal-formatted props, the table should load with errors", () => {
    const localVue = createLocalVue();
    let wrapper = mount(ServicesDeliveredTable, {
      localVue,
      i18n,
      store,
      vuetify: new Vuetify(),
      propsData: {
        parsed_value: [
          { date: "s019-10-05" },
          { starttime: "09:00AM" },
          { endtime: "1000 PM" },
          { totalHours: "11.00" },
        ],
        cols: valCols,
      },
    });
    const expectedAmtErrors = 16; // 1 row with 4/5 invalid fields
    wrapper.vm.initialize();
    amtErrors = wrapper.vm.validate();

    expect(wrapper.vm.allEntries.length).toBe(4);
    expect(wrapper.vm.amtEdited).toBe(0);
    if (amtErrors !== expectedAmtErrors) {
      wrapper.vm.printErrors();
    }
    expect(amtErrors).toBe(expectedAmtErrors);
    wrapper.destroy();
  });
});

describe("ServicesDeliveredTable.js", () => {
  let conflicting_props = {};
  conflicting_props["parsed_value"] = conflicting_timesheet["timesheet"];
  conflicting_props["value"] = conflicting_timesheet["timesheet"];
  conflicting_props["disabled"] = true;
  conflicting_props["cols"] = valCols;

  // Valid entries that conflict should return an error
  it("Given invalid props, the table should load with errors", () => {
    const localVue = createLocalVue();
    let wrapper = mount(ServicesDeliveredTable, {
      localVue,
      i18n,
      store,
      vuetify: new Vuetify(),
      propsData: conflicting_props,
    });
    expect(wrapper.vm.allEntries.length).toBe(2);
    expect(wrapper.vm.amtEdited).toBe(0);

    amtErrors = wrapper.vm.validate();
    const expectedAmtErrors = 2; // 2 valid rows, but conflicting time
    if (amtErrors !== expectedAmtErrors) {
      wrapper.vm.printErrors();
    }
    expect(amtErrors).toBe(expectedAmtErrors);
    expect(wrapper.vm.checkOverlapping()).toBe(1);
    wrapper.destroy();
  });
});
