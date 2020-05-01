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

	//Checks if there is a file, and the "Complete timesheet" button was clicked, the loader opens.
	/*
	it('checks loader is called when complete timesheet button is clicked' () => {
		let mockFile = Object.create(null)
		wrapper = shallowMount (FileUpload, {
			propsData: {
				files: [mockFile]
			}
		})
*/

})
