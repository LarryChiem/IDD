<template>
  <v-form class="mx-9" lazy-validation ref="form" v-model="valid">
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

    <v-container>
      <v-row>
        <v-col>
          <v-btn color="error" class="mr-4" @click="reset">
            Reset Form
          </v-btn>
        </v-col>

        <v-col>
          <ConfirmSubmission
            :valid="valid"
            :errors="errors"
            :formFields="formFields"
            :totalEdited="totalEdited"
            :validationSignal="validationSignal"
            @click="validateInputs"
          />
        </v-col>
      </v-row>
    </v-container>
  </v-form>
</template>

<script>
  import FormTable from "@/components/Timesheet/FormTable";
  import FormField from "@/components/Timesheet/FormField";
  import ConfirmSubmission from "@/components/Timesheet/ConfirmSubmission";
  import fieldData from "@/components/Timesheet/IDDFormFields.json";
  import rules from "@/components/Timesheet/FormRules.js";
  import time_functions from "@/components/Timesheet/TimeFunctions.js";

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
          var _rules = value.rules;
          this.$set(fieldData[key], "rules", []);
          _rules.forEach((fieldRule) => {
            // Not using the spread operator for IE compatibility
            fieldData[key].rules.push.apply(
              fieldData[key].rules,
              rules[fieldRule]
            );
          });

          if (fieldData[key].counter) {
            fieldData[key].rules.push(rules.maxLength(fieldData[key].counter));
          }
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

        // The number of invalid fields
        errors: [],

        // Signal denoting completion of validation for form fields
        validationSignal: false,
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

        // Consider the amount of non-parsed fields
        // The provider and employer must resign the form
        Object.entries(this.formFields).forEach(([key, value]) => {
          key;
          if (!("parsed_value" in value)) {
            this.totalEdited += 1;
          }
        });
      },

      // Check if the form fields have valid input
      validateInputs() {
        this.validate(); 

        // Change this field to validation signal ConfirmSubmission
        this.validationSignal = !this.validationSignal;
      },

      // Compute the sum of all serviceDeliveredOn totalHours with the totalHours field
      sumTableHours() {
        var sumHours = 0;
        var sumMinutes = 0;

        // For each row in the array of entries...
        this.formFields["serviceDeliveredOn"]["value"].forEach((entry) => {
          // Check that the totalHours field is valid
          if (entry["errors"]["totalHours"].length == 0) {
            sumHours += parseInt(entry["totalHours"].substr(0, 2));
            sumMinutes += parseInt(entry["totalHours"].substr(3, 2));
          }
        });
        sumHours += (sumMinutes - (sumMinutes % 60)) / 60;
        sumMinutes %= 60;

        return sumHours.toString() + ":" + sumMinutes.toString();
      },

      // Count the number of errors in the serviceDeliveredOn table
      getTableErrors() {
        // For each row in the array of entries...
        this.formFields["serviceDeliveredOn"]["value"].forEach(
          (entry, index) => {
            // For each error col in an entry, check the amount of errors
            Object.entries(entry["errors"]).forEach(([col, errors]) => {
              var colErrors = errors.length;
              if (colErrors > 0) {
                this.errors.push(
                  [`ERROR: in row ${
                    index + 1
                  } of the serviceDeliveredOn table, '${col}' has the following errors:`,
                  errors]
                );
              }
            });
          }
        );
      },

      // Validate the form
      validate() {
        // Reset all error messages and validation
        this.errors = [];

        // Check parent's response on validity of input fields
        if (!this.$refs.form.validate()) {
          this.errors.push("ERROR: Invalid input in some form fields!");
        }


        // Check the validity of the serviceDeliveredOn table
        this.getTableErrors();

        // Ensure that the serviceDeliveredOn table sum == totalHours field
        if (this.formFields.totalHours.value !== null) {
          var sumHours = this.sumTableHours();
          if (sumHours.localeCompare(this.formFields.totalHours.value) !== 0) {
            this.errors.push(
              `ERROR: valid rows in the serviceDeliveredOn table sums up to ${sumHours} hours, but the totalHours field reports ${this.formFields.totalHours.value} hours!`
            );
          }
        }

        // If there were no edited fields, ensure that the provider and
        // employer signature date are after the last service date
        if (this.totalEdited <= 0) {
          // Only compare the earlier date
          var comparisonDate = this.formFields.providerSignDate.value;
          if (
            time_functions.dateCompare(
              comparisonDate,
              this.formFields.employerSignDate.value
            ) > 0
          ) {
            comparisonDate = this.formFields.employerSignDate.value;
          }

          // Compare signage dates with the pay period
          // Note, only comparing the YYYY-mm part
          var submissionDate = this.formFields.submissionDate.value;
          if (
            time_functions.dateCompare(
              comparisonDate.substr(0, 7),
              submissionDate
            ) < 0
          ) {
            this.errors.push(
              `ERROR: the employer or provider sign date is before the pay period.`
            );
          }

          // Get the last date from the serviceDeliveredOn table
          var latestDateIdx = this.formFields.serviceDeliveredOn.value.length;
          if (latestDateIdx > 0) {
            var latestDate = this.formFields.serviceDeliveredOn.value[
              latestDateIdx - 1
            ]["date"];
            if (time_functions.dateCompare(comparisonDate, latestDate) < 0) {
              this.errors.push(
                `ERROR: the employer or provider sign date is before the latest service delivery date.`
              );
            }
          }
        }
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
      },
    },
  };
</script>
