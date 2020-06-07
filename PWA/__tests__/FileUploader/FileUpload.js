import { shallowMount, mount } from "@vue/test-utils";
import sinon from "sinon";
import Vue from "vue";
import Vuetify from "vuetify";
import i18n from '@/plugins/i18n';
import store from "@/store/index.js";
import FileUpload from "@/components/Forms/FileUploader.vue";

Vue.use(Vuetify, i18n);

describe("FileUpload", () => {
  //Checks if the upload status is not active since no files to upload
  it("upload status should be zero since no files", () => {
    const wrapper = mount(FileUpload, {
      store,
      i18n,
      propsData: {
        files: [],
      },
    });
    expect(wrapper.vm.files.length).toBe(0);
  });
});
