import { shallowMount, mount } from "@vue/test-utils";
import sinon from "sinon";
import Vue from "vue";
import Vuetify from "vuetify";
import store from "@/store/index.js";
import FileUpload from "@/components/Forms/FileUploader.vue";

Vue.use(Vuetify);

describe("FileUpload", () => {
  //Checks if the upload status is not active since no files to upload
  it("upload status should be zero since no files", () => {
    const wrapper = mount(FileUpload, {
      store,
      propsData: {
        files: [],
      },
    });
    expect(wrapper.vm.files.length).toBe(0);
  });
});
