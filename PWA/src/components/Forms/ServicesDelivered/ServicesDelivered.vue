<template>
  <v-form class="mx-2" lazy-validation ref="form" v-model="valid">
    <p class="title">
      {{ $t('components_Forms_ServicesDelivered_front') }}
    </p>

    <v-layout wrap>
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
        v-bind="combineProps([fieldProps[field], formFields[field]])"
        :label="$t(fieldProps[field].label)"
        :hint="$t(fieldProps[field].hint)"
        :willResign="willResign"
        :key="field"
        :reset="resetChild"
        @disable-change="handleDisableChange(field, $event)"
        @input="setField(wrapSet('formFields.' + field + '.value', $event))"
      />
    </v-layout>

    <!-- Table containing timesheet  -->
    <v-card-text>
      <ServicesDeliveredTable
        v-bind="formFields.timesheet"
        :label="$t(fieldProps['totalHours'].label)"
        :cols="valCols"
        :value="formFields.timesheet.value"
        :reset="resetChild"
        :totalHours="formFields['totalHours']['value']"
        :willResign="willResign"
        @update-totalHours="
          setField(wrapSet('formFields.totalHours.value', $event))
        "
        @disable-change="handleDisableChange('timesheet', $event)"
        @input="
          setField(
            wrapSet(
              'formFields.timesheet.value',
              JSON.parse(JSON.stringify($event))
            )
          )
        "
      />
    </v-card-text>

    <!-- totalHours -->
    {{ $t('components_Forms_ServicesDelivered_totalhours') }}
    {{ this.formFields["totalHours"]["value"] }}
    <hr />

    <p class="title">
      {{ $t('components_Forms_ServicesDelivered_back') }}
    </p>

    <FormField
      v-for="field in ['serviceGoal', 'progressNotes']"
      v-bind="combineProps([fieldProps[field], formFields[field]])"
      :label="$t(fieldProps[field].label)"
      :hint="$t(fieldProps[field].hint)"
      :value="formFields[field].value"
      :willResign="willResign"
      :key="field"
      :reset="resetChild"
      @disable-change="handleDisableChange(field, $event)"
      @input="setField(wrapSet('formFields.' + field + '.value', $event))"
    />

    <hr />

    <!-- Employer Verification Section -->
    <p class="subtitle-1"
      v-html="$t('components_Forms_ServicesDelivered_employer_verification')"
    ></p>

    <v-layout wrap>
      <FormField
        v-for="field in ['employerSignature', 'employerSignDate']"
        v-bind="combineProps([fieldProps[field], formFields[field]])"
        :label="$t(fieldProps[field].label)"
        :hint="$t(fieldProps[field].hint)"
        :value="formFields[field].value"
        :willResign="willResign"
        :key="field"
        :reset="resetChild"
        @disable-change="handleDisableChange(field, $event)"
        @input="setField(wrapSet('formFields.' + field + '.value', $event))"
      />
    </v-layout>
    <!-- End Employer Verification Section -->

    <hr />

    <!-- Provider Verification Section -->
    <p class="subtitle-1"
      v-html="$t('components_Forms_ServicesDelivered_provider_verification')"
    ></p>

    <v-layout wrap>
      <FormField
        v-for="field in ['providerSignature', 'providerSignDate']"
        v-bind="combineProps([fieldProps[field], formFields[field]])"
        :label="$t(fieldProps[field].label)"
        :hint="$t(fieldProps[field].hint)"
        :value="formFields[field].value"
        :willResign="willResign"
        :key="field"
        :reset="resetChild"
        @disable-change="handleDisableChange(field, $event)"
        @input="setField(wrapSet('formFields.' + field + '.value', $event))"
      />
    </v-layout>
    <!-- END Provider Verification Section -->

    <hr />
    <v-layout wrap>
      <FormField
        v-for="field in ['authorization', 'approval']"
        v-bind="combineProps([fieldProps[field], formFields[field]])"
        :label="$t(fieldProps[field].label)"
        :value="formFields[field].value"
        :willResign="willResign"
        :key="field"
        :reset="resetChild"
        @disable-change="handleDisableChange(field, $event)"
        @input="setField(wrapSet('formFields.' + field + '.value', $event))"
      />
    </v-layout>

    <strong class="subtitle-1">
      {{ $t('components_Forms_ServicesDelivered_authorize') }}
    </strong>

    <v-container>
      <v-row>
        <v-col cols="6">
          <v-container class="text-center">
            <v-btn color="error" class="mr-4" @click="reset">
              {{ $t('components_Forms_ServicesDelivered_reset') }}
            </v-btn>
          </v-container>
        </v-col>
        <v-col cols="6">
          <ConfirmSubmission
            :cols="cols"
            :valid="valid"
            :errors="errors"
            :validationSignal="validationSignal"
            @click="validateInputs"
          />
        </v-col>
      </v-row>
    </v-container>
  </v-form>
</template>

<script>
  import i18n from '@/plugins/i18n';
  import ServicesDeliveredTable from "@/components/Forms/ServicesDelivered/ServicesDeliveredTable";
  import FormField from "@/components/Forms/FormField";
  import ConfirmSubmission from "@/components/Forms/ConfirmSubmission";
  import fieldPropsFile from "@/components/Forms/ServicesDelivered/ServicesDeliveredFields.json";
  import rules from "@/components/Utility/FormRules.js";
  import { TIME } from "@/components/Utility/Enums.js";
  import { subtractTime, isValid } from "@/components/Utility/TimeFunctions.js";

  import { mapFields } from "vuex-map-fields";
  import { mapMutations } from "vuex";

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
    },

    // Upon first loading on the page, bind parsed form data to each
    // IDD Timesheet form field
    created: function () {
      // If the user is working with new data, prepare the store
      if (this.newForm === true) {
        this.bindData();
        this.initialize();
        this.set(this.wrapSet("newForm", false));
      }

      // Bind validation rules to each field that has a 'rules' string
      // specified
      Object.entries(this.fieldProps).forEach(([key, value]) => {
        if ("rules" in value) {
          var _rules = value.rules;
          let _transRules = [];
          _rules.forEach((fieldRule) => {
            if (typeof fieldRule === "string") {
              _transRules.push(rules[fieldRule]());
            }
          });

          if (this.fieldProps[key].counter !== undefined) {
            _transRules.push(rules.maxLength(this.fieldProps[key].counter));
          }
          this.$set(this.fieldProps[key], "rules", _transRules);
        }
      });
    },

    data: function () {
      return {
        // Reset form of arbitrary value
        resetChildField: false,

        // Hide form validation error messages by default
        valid: true,

        // The number of invalid fields
        errors: [],

        cols: ["date", "starttime", "endtime", "totalHours", "group"],
        valCols: ["date", "starttime", "endtime", "totalHours"],

        // Expose the field props, so we can reference it in the HTML
        fieldProps: JSON.parse(JSON.stringify(fieldPropsFile)),

        // Signal denoting completion of validation for form fields
        validationSignal: false,
      };
    },

    computed: {
      resetChild() {
        return this.resetChildField;
      },
      ...mapFields(["formId", "newForm"]),
      ...mapFields("ServiceDelivered", [
        "willResign",
        "formFields",
        "totalEdited",
      ]),
    },
    methods: {
      // Expose and rename the mutations for changing vuex state
      ...mapMutations({
        incrementEdited: "ServiceDelivered/incrementEdited",
        setField: "ServiceDelivered/updateField",
        setTimesheet: "ServiceDelivered/updateTimesheet",
        set: "updateField",
      }),
      bindData() {
        // Bind data from a .json IDD timesheet to forFields in the vuex store
        if (this.parsedFileData !== null) {
          Object.entries(this.parsedFileData).forEach(([key, value]) => {
            if (key in this.formFields) {
              this.setField(
                this.wrapSet("formFields." + key + ".parsed_value", value)
              );
            } else {
              if (
                key.localeCompare("id") !== 0 &&
                key.localeCompare("review_status") !== 0
              ) {
                console.log(
                  "Unrecognized parsed form field from server: " +
                    `${key} - ${value}`
                );
              }
            }
          });
        }
      },
      combineProps(props) {
        return Object.assign(...props);
      },
      initialize() {
        // Initialize some fields
        this.set(this.wrapSet("willResign", false));

        // Set values to their parsed parts or to empty
        Object.entries(this.formFields).forEach(([key, value]) => {
          if (key in this.formFields) {
            // Set values to their parsed part, or keep as default value
            const parsed_val = this.formFields[key]["parsed_value"];
            if (parsed_val !== null) {
              this.setField(
                this.wrapSet("formFields." + key + ".value", parsed_val)
              );
              this.setField(
                this.wrapSet("formFields." + key + ".disabled", true)
              );
            }
          } else {
            if (
              key.localeCompare("id") !== 0 &&
              key.localeCompare("review_status") !== 0
            ) {
              console.log(
                "Unrecognized parsed form field from vuex store: " +
                  `${key} - ${value}`
              );
            }
          }
        });

        // Consider the amount of non-parsed fields
        // The provider and employer must resign the form
        Object.entries(this.formFields).forEach(([key, value]) => {
          key;
          if (!("parsed_value" in value)) {
            this.incrementEdited(1);
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
          if ("errors" in entry) {
            Object.entries(entry["errors"]).forEach(([col, errors]) => {
              var colErrors = errors.length;
              if (colErrors > 0) {
                this.errors.push([
                  i18n.t('components_Forms_ServicesDelivered_err5_0') +
                  `${index + 1}` + 
                  i18n.t('components_Forms_ServicesDelivered_err5_1') +
                  `${col}` +
                  i18n.t('components_Forms_ServicesDelivered_err5_2') +
                  errors,
                ]);
              }
            });
          }
        });
      },

      // Validate the form
      validate() {
        // Reset all error messages and validation
        this.errors = [];

        // Check parent's response on validity of input fields
        if (!this.$refs.form.validate()) {
          this.errors.push(i18n.t('components_Forms_ServicesDelivered_err1'));
        }

        // Check the validity of the timesheet table
        this.getTableErrors();

        // If there were no edited fields, ensure that the provider and
        // employer signature date are after the last service date
        if (this.totalEdited <= 0) {
          if (
            isValid(
              this.formFields.providerSignDate.value,
              TIME.YEAR_MONTH_DAY
            ) === true &&
            isValid(
              this.formFields.employerSignDate.value,
              TIME.YEAR_MONTH_DAY
            ) === true
          ) {
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
              this.errors.push(i18n.t('components_Forms_ServicesDelivered_err6'));
            }

            // Get the last date from the timesheet table
            var latestDateIdx = this.formFields.timesheet.value.length;
            if (latestDateIdx > 0) {
              var latestDate = this.formFields.timesheet.value[
                latestDateIdx - 1
              ]["date"];
              if (
                subtractTime(latestDate, comparisonDate, TIME.YEAR_MONTH_DAY) <
                0
              ) {
                this.errors.push(i18n.t('components_Forms_ServicesDelivered_err7'));
              }
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
            this.setField(
              this.wrapSet("formFields[" + field + "].disabled", true)
            );
            this.setField(
              this.wrapSet(
                "formFields[" + field + "].value",
                this.formFields[field].parsed_value
              )
            );
          }
        }
      },

      // Update this component's disabled property
      // Then, update the amount of parsed fields edited
      handleDisableChange(fieldName, amtEdited) {
        if (amtEdited > 0) {
          this.setField(
            this.wrapSet("formFields[" + fieldName + "].disabled", false)
          );
          this.setField(this.wrapSet("willResign", true));
        } else {
          this.setField(
            this.wrapSet("formFields[" + fieldName + "].disabled", true)
          );
        }
        this.incrementEdited(amtEdited);
      },

      wrapSet(path, value) {
        return { path: path, value: value };
      },
    },
  };
</script>
