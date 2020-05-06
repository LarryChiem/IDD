import {mount} from '@vue/test-utils'
import ConfirmSubmission from '../../src/components/Timesheet/ConfirmSubmission.vue'

describe('ConfirmSubmission', () => {

	//Given a valid form, the user should not get an error.
	it('Should not load message saying invalid form', () => {

		const wrapper = mount(ConfirmSubmission, {
			propsData: {
				valid: true,
				formFields: null,
			}
		})

		expect(wrapper.find('#invalid').exists()).toBe(false)
	})

	//Given invalid form, it should notify the user.
	it('Should load message saying invalid form', () => {

		const wrapper = mount(ConfirmSubmission, {
			propsData: {
				valid: false,
				formFields: null,
			}
		})

		expect(wrapper.find('#invalid').exists()).toBe(true)
	})

	//Given an valid form, and submit has been clicked, it should prompt them if they are sure they
	//want to submit.
	it('Should ask if user is sure they want to submit given valid form', () => {
		const wrapper = mount(ConfirmSubmission, {
			propsData: {
				valid: true,
				formFields: null,
			},
		})
		wrapper.setData({ loading: false})

		expect(wrapper.find('#confirm').exists()).toBe(true)

		})

	})

