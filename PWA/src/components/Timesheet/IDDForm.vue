<template>
  <v-form
    class="mx-9"
    lazy-validation
    ref="form"
    v-model="valid"
  >

    <p class="title">
      Front side of the form
    </p>

    <FormField
      v-for="field in [
        'customerName',
        'prime',
        'submissionDate',
        'providerName',
        'providerNumber',
        'SC/PA Name',
        'CMOrg',
        'service',
      ]"
      v-model="formFields[field].value"
      v-bind="formFields[field]"
      :key="field"
      :reset="resetChild"
      @disable-change="handleDisableChange(field, $event)"
    />

    <!-- Table containing timesheet  -->
    <v-card-text>
      <FormTable
        v-model="formFields.serviceDeliveredOn.value"
        v-bind="formFields.serviceDeliveredOn"
        :reset="resetChild"
        @disable-change="handleDisableChange('serviceDeliveredOn', $event)"
      />
    </v-card-text>

    <!-- totalHours -->
    <FormField
      v-model="formFields.totalHours.value"
      v-bind="formFields.totalHours"
      :reset="resetChild"
      @disable-change="handleDisableChange('totalHours', $event)"
    />

    <hr />

    <p class="title">
      Back side of the form
    </p>

    <FormField
      v-for="field in ['serviceGoal', 'progressNotes']"
      v-model="formFields[field].value"
      v-bind="formFields[field]"
      :key="field"
      :reset="resetChild"
      @disable-change="handleDisableChange(field, $event)"
    />

    <hr />

    <!-- Employer Verification Section -->
    <p class="subtitle-1">
      <strong>RECIPIENT/EMPLOYER VERIFICATION:</strong><br />
      <em>
        I affirm that the data reported on this form is for actual dates/time
        worked by the provider delivering the service/supports listed to the
        recipient, that it does not exceed the total amount of service
        authorized and was delivered according to the recipient's service plan
        and provider/recipient service agreement.
      </em>
    </p>

    <FormField
      v-for="field in ['employerSignature', 'employerSignDate']"
      v-model="formFields[field].value"
      v-bind="formFields[field]"
      :key="field"
      :reset="resetChild"
      @disable-change="handleDisableChange(field, $event)"
    />
    <!-- End Employer Verification Section -->

    <hr />

    <!-- Provider Verification Section -->
    <p class="subtitle-1">
      <strong>PROVIDER/EMPLOYEE VERIFICATION:</strong><br />
      <em>
        I affirm that the data reported on this form is for actual dates/time I
        worked delivering the service/supports listed to the recipient, that it
        does not exceed the total amount of service authorized and was delivered
        according to the recipient's service plan and provider/recipient service
        agreement. I further acknowledge that reporting dates/tim worked in
        excess of the amount of service authorized or not consistent with the
        recipient's service plan may be considered Medicaid Fraud.
      </em>
    </p>

    <FormField
      v-for="field in ['providerSignature', 'providerSignDate']"
      v-model="formFields[field].value"
      v-bind="formFields[field]"
      :key="field"
      :reset="resetChild"
      @disable-change="handleDisableChange(field, $event)"
    />
    <!-- END Provider Verification Section -->

    <hr />

    <strong class="subtitle-1">
      <FormField
        v-for="field in ['authorization', 'providerInitials']"
        v-model="formFields[field].value"
        v-bind="formFields[field]"
        :key="field"
        :reset="resetChild"
        @disable-change="handleDisableChange(field, $event)"
      />

      Providers submit this completed/signed form to the CDDP, Brokerage or CIIS
      Program that authorized the service delivered.
    </strong>

    <hr />

		<v-container>
			<v-row>
				<v-col>
					<v-btn
						color="error"
						class="mr-4"
						@click="reset"
					>
						Reset Form
					</v-btn>
				</v-col>

				<v-col>
					<ConfirmSubmission
						v-bind:valid="valid"
						v-bind:formFields="formFields"/>
				</v-col>
			</v-row>
		</v-container>

		<!--
    <v-btn color="success" class="mr-4" :disabled="!valid" @click="submit">
      Submit
    </v-btn>
		-->
  </v-form>
</template>

<script>
import FormTable from "@/components/Timesheet/FormTable";
import FormField from "@/components/Timesheet/FormField";
import fieldData from "@/components/Timesheet/IDDFormFields.json";
import rules from "@/components/Timesheet/FormRules.js";
import ConfirmSubmission from "@/components/Timesheet/ConfirmSubmission";

export default {
  name: "IDDForm",
  components: {
    FormTable,
    FormField,
		ConfirmSubmission,
  },

  props: {
    // A .json file that is the parsed uploaded IDD timesheet data
    parsedFileData: {
      type: Object,
      default: null,
    },
  },

  // Upon first loading on the page, bind parsed form data to each
  // IDD Timesheet form field
  created: function () {
    this.initialize();

    // Bind validation rules to each field that has a 'rules' string
    // specified
    Object.entries(fieldData).forEach(([key, value]) => {
      if ("rules" in value) {
        fieldData[key].rules = rules[fieldData[key].rules];
      }
    });
  },

  data: function () {
    return {
      // Import form field structure data and store into local variable
      formFields: fieldData,

      // Reset form of arbitrary value
      resetChildField: false,

      // The amount of parsed fields that were edited
      totalEdited: 0,

      // Hide form validation error messages by default
      valid: true,

    };
  },

  computed: {
    resetChild() {
      return this.resetChildField;
    },
  },

  methods: {
    initialize() {
      // Initialize some fields
      this.totalEdited = 0;

      // Bind data from a .json IDD timesheet to form fields
      if (this.entries !== null) {
        Object.entries(this.parsedFileData).forEach(([key, value]) => {
          if (key in this.formFields) {
            this.formFields[key]["parsed_value"] = value;
            this.formFields[key]["value"] = value;
            this.formFields[key]["disabled"] = true;
          } else {
            console.log(
              "Unrecognized parsed form field from server: " +
                `${key} - ${value}`
            );
          }
        });
      }
    },

    validateFormTable(hours) {
      // TODO - Compare form table total hours with parsed total hours
      hours;
    },

    submit() {
      this.$refs.form.validate();
    },

    reset() {
      // re-initialize values
      this.initialize();
      this.resetValidation();

      // Change the value of this watched prop to force
      // FormField components to reset
      this.resetChildField = !this.resetChildField;
    },

    resetValidation() {
      this.$refs.form.resetValidation();
    },

    // For a parsed field, send a warning if being edited
    // Else, reset value to parsed value & disable field
    resetParsed(field) {
      if (this.formFields[field].parsed_value !== undefined) {
        if (this.formFields[field].disabled !== true) {
          this.formFields[field].value = this.formFields[field].parsed_value;
          this.formFields[field].disabled = true;
        }
      }
    },

    // Update this component's disabled property
    // Then, update the amount of parsed fields edited
    handleDisableChange(fieldName, amtEdited) {
      if (amtEdited > 0) {
        this.formFields[fieldName].disabled = true;
      } else {
        this.formFields[fieldName].disabled = false;
      }
      this.totalEdited += amtEdited;
      console.log(fieldName, amtEdited, "->", this.totalEdited);
    },
  },
};
</script>
