import { mount } from "@vue/test-utils";
import { shallowMount } from "@vue/test-utils";
import Vue from "vue";
import Vuetify from "vuetify";
import store from "@/store/index.js";
import ConfirmSubmission from "@/components/Forms/ConfirmSubmission.vue";

Vue.use(Vuetify);

describe("ConfirmSubmission", () => {
  //Given a valid form, the user should not get an error.
  it("Should not load message saying invalid form", () => {
    const wrapper = mount(ConfirmSubmission, {
      store,
      propsData: {
        valid: true,
        formFields: null,
      },
    });
    expect(wrapper.find("#invalid").exists()).toBe(false);
    wrapper.destroy();
  });

  //Given invalid form, it should notify the user.
  it("Should load message saying invalid form", () => {
    const wrapper = shallowMount(ConfirmSubmission, {
      store,
      propsData: {
        valid: false,
        formFields: null,
      },
    });

    expect(wrapper.find("#invalid").exists()).toBe(true);
    wrapper.destroy();
  });

  //Given a valid form, and submit has been clicked, it should prompt them if they are sure they
  //want to submit.
  it("Should ask if user is sure they want to submit given valid form", () => {
    const wrapper = shallowMount(ConfirmSubmission, {
      store,
      propsData: {
        valid: true,
        formFields: null,
      },
    });
    wrapper.setData({ displaySubmit: true });
    wrapper.setData({ loading: false });

    expect(wrapper.find("#confirm").exists()).toBe(true);
    wrapper.destroy();
  });
});
