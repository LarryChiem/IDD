import { shallowMount, mount } from "@vue/test-utils";
import sinon from "sinon";
import Vue from "vue";
import Vuetify from "vuetify";
import FileUpload from "@/components/Forms/FileUploader.vue";

Vue.use(Vuetify);

describe("FileUpload", () => {
  //Checks if the upload status is not active since no files to upload
  it("upload status should be zero since no files", () => {
    const wrapper = mount(FileUpload, {
      propsData: {
        files: [],
      },
    });
    expect(wrapper.vm.files.length).toBe(0);
  });
});
