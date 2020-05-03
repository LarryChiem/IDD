import {shallowMount} from '@vue/test-utils'
import FileUpload from '../../src/components/Timesheet/FileUploader.vue'
import { mount } from '@vue/test-utils'
import sinon from 'sinon'

describe('FileUpload', () => {

	//Checks if the upload status is not active since no files to upload
	it('upload status should be zero since no files', () => {
		const wrapper = mount(FileUpload, {
			propsData: {
				files: []
			}
		})
		expect(wrapper.vm.files.length).toBe(0)
})

})
