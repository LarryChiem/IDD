<template>
  <v-form class="mx-9" lazy-validation ref="form" v-model="valid">
    <p class="title">
      Front side of the form
    </p>

    <FormField
      v-for="field in [
        'clientName',
        'prime',
        'submissionDate',
        'providerName',
        'providerNum',
        'scpaName',
        'brokerage',
        'serviceAuthorized',
      ]"
      v-model="formFields[field].value"
      v-bind="formFields[field]"
      :willResign="willResign"
      :key="field"
      :reset="resetChild"
      @disable-change="handleDisableChange(field, $event)"
    />

    <!-- Table containing timesheet  -->
    <v-card-text>
      <ServicesDeliveredTable
        :cols="valCols"
        v-model="formFields.timesheet.value"
        v-bind="formFields.timesheet"
        :reset="resetChild"
        :totalHours="formFields['totalHours']['value']"
        :willResign="willResign"
        @update-totalHours="formFields['totalHours']['value'] = $event"
        @disable-change="handleDisableChange('timesheet', $event)"
      />
    </v-card-text>

    <!-- totalHours -->
    Total Hours:
    {{ this.formFields["totalHours"]["value"] }}
    <hr />

    <p class="title">
      Back side of the form
    </p>

    <FormField
      v-for="field in ['serviceGoal', 'progressNotes']"
      v-model="formFields[field].value"
      v-bind="formFields[field]"
      :willResign="willResign"
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
      :willResign="willResign"
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
      :willResign="willResign"
      :key="field"
      :reset="resetChild"
      @disable-change="handleDisableChange(field, $event)"
    />
    <!-- END Provider Verification Section -->

    <hr />

    <strong class="subtitle-1">
      <FormField
        v-for="field in ['authorization', 'approval']"
        v-model="formFields[field].value"
        v-bind="formFields[field]"
        :willResign="willResign"
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
            :cols="cols"
            :valid="valid"
            :errors="errors"
            :formFields="formFields"
            :totalEdited="totalEdited"
            :validationSignal="validationSignal"
            :formID="formID"
            :formChoice="formChoice"
            @click="validateInputs"
          />
        </v-col>
      </v-row>
    </v-container>
  </v-form>
</template>

<script>
  import ServicesDeliveredTable from "@/components/Forms/ServicesDelivered/ServicesDeliveredTable";
  import FormField from "@/components/Forms/FormField";
  import ConfirmSubmission from "@/components/Forms/ConfirmSubmission";
  import fieldData from "@/components/Forms/ServicesDelivered/ServicesDeliveredFields.json";
  import rules from "@/components/Utility/FormRules.js";
  import { TIME } from "@/components/Utility/Enums.js";
  import { subtractTime } from "@/components/Utility/TimeFunctions.js";

  export default {
    name: "ServicesDelivered",
    components: {
      ServicesDeliveredTable,
      FormField,
      ConfirmSubmission,
    },

    props: {
      // A .json file that is the parsed uploaded IDD timesheet data
      parsedFileData: {
        type: Object,
        default: null,
      },
      formChoice: {
        type: Number,
      },
    },

    // Upon first loading on the page, bind parsed form data to each
    // IDD Timesheet form field
    created: function () {
      this.formID = this.parsedFileData["id"];
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

        // The unique ID associated with this form
        formID: 0,

        // Hide form validation error messages by default
        valid: true,

        // The number of invalid fields
        errors: [],

        cols: ["date", "starttime", "endtime", "totalHours", "group"],
        valCols: ["date", "starttime", "endtime", "totalHours"],

        // Signal denoting completion of validation for form fields
        validationSignal: false,

        willResign: false,
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
        this.willResign = false;

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

      // Count the number of errors in the timesheet table
      getTableErrors() {
        // For each row in the array of entries...
        this.formFields["timesheet"]["value"].forEach((entry, index) => {
          // For each error col in an entry, check the amount of errors
          Object.entries(entry["errors"]).forEach(([col, errors]) => {
            var colErrors = errors.length;
            if (colErrors > 0) {
              this.errors.push([
                `ERROR: in row ${
                  index + 1
                } of the timesheet table, '${col}' has the following errors:`,
                errors,
              ]);
            }
          });
        });
      },

      // Validate the form
      validate() {
        // Reset all error messages and validation
        this.errors = [];

        // Check parent's response on validity of input fields
        if (!this.$refs.form.validate()) {
          this.errors.push("ERROR: Invalid input in some form fields!");
        }

        // Check the validity of the timesheet table
        this.getTableErrors();

        // If there were no edited fields, ensure that the provider and
        // employer signature date are after the last service date
        if (this.totalEdited <= 0) {
          // Only compare the earlier date
          var comparisonDate = this.formFields.providerSignDate.value;
          if (
            subtractTime(
              comparisonDate,
              this.formFields.employerSignDate.value,
              TIME.YEAR_MONTH_DAY
            ) < 0
          ) {
            comparisonDate = this.formFields.employerSignDate.value;
          }

          // Compare signage dates with the pay period
          var submissionDate = this.formFields.submissionDate.value;
          var submissionDiff = subtractTime(
            comparisonDate.substr(0, 7),
            submissionDate,
            TIME.YEAR_MONTH
          );
          if (submissionDiff < 0) {
            this.errors.push(
              `ERROR: the employer or provider sign date is before the pay period.`
            );
          }

          // Get the last date from the timesheet table
          var latestDateIdx = this.formFields.timesheet.value.length;
          if (latestDateIdx > 0) {
            var latestDate = this.formFields.timesheet.value[latestDateIdx - 1][
              "date"
            ];
            if (
              subtractTime(latestDate, comparisonDate, TIME.YEAR_MONTH_DAY) < 0
            ) {
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
          this.willResign = true;
        } else {
          this.formFields[fieldName].disabled = false;
        }
        this.totalEdited += amtEdited;
      },
    },
  };
</script>
