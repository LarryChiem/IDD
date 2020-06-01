import Vue from "vue";
import Vuetify from "vuetify";
import i18n from '@/plugins/i18n';
import ServicesDelivered from "@/components/Forms/ServicesDelivered/ServicesDelivered.vue";
import happy_path from "@/components/Utility/happy_path.json";
import store from "@/store/index.js";
import { FORM } from "@/components/Utility/Enums.js";

import { mount, createLocalVue } from "@vue/test-utils";

Vue.use(Vuetify, i18n);

const valCols = ["date", "starttime", "endtime", "totalHours"];
let amtErrors = 0;

describe("ServicesDelivered.js", () => {
  // Given the happy path, the form should have no errors
  it("Given a happy path submission, the form should load with no errors", () => {
    const localVue = createLocalVue();
    let wrapper = mount(ServicesDelivered, {
      localVue,
      i18n,
      store,
      vuetify: new Vuetify(),
      propsData: {
        parsedFileData: happy_path,
        formChoice: FORM.OR507_RELIEF,
      },
    });
    wrapper.vm.validate();
    expect(wrapper.vm.errors.length).toBe(0);
    wrapper.destroy();
  });
});
