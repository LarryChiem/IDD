<template>
  <v-container class="text-center">
    <v-btn color="success" dark @click.stop="signalParentValidate">
      Submit
    </v-btn>

    <v-dialog v-model="displaySubmit">
      <div v-if="isValid">
        <v-card>
          <div v-if="!loading">
            <v-card-title class="headline" id="confirm">
              Are you sure want to submit the form?
            </v-card-title>

            <v-card-text>
              Some text talking about how this submission is final unless
              something is wrong with it.
            </v-card-text>

            <v-card-text v-if="this.totalEdited > 0">
              <em>
                There were {{ totalEdited }} edited fields. Provider and
                Employer must resign the form.
              </em>

              <!-- 
                If there were any changes to the form, disable 
                submission until both provider and employer re-sign. 
              -->
              <v-container fluid>
                <v-checkbox
                  color="success"
                  v-model="reSigned"
                  label="Employer accepts all changes to the IDD form."
                  value="Employer"
                ></v-checkbox>
                <v-checkbox
                  color="success"
                  v-model="reSigned"
                  label="Provider accepts all changes to the IDD form."
                  value="Provider"
                ></v-checkbox>
              </v-container>

              <v-card
                color="red lighten-2"
                v-if="!reSigned.includes('Employer')"
              >
                Employer must re-sign form!
              </v-card>
              <v-card
                color="red lighten-2"
                v-if="!reSigned.includes('Provider')"
              >
                Provider must re-sign form!
              </v-card>
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>

              <!-- Confirm if user is ready to submit -->
              <v-btn color="red" text @click="displaySubmit = false">
                Cancel
              </v-btn>

              <template v-if="onlineStatus">
                <v-btn
                  text
                  color="green darken-1"
                  :disabled="canSubmit"
                  @click="submit"
                >
                  Submit
                </v-btn>
              </template>
              <template v-else>
                Offline! Please connect to the internet to submit.
              </template>
            </v-card-actions>
          </div>

          <div v-else>
            <div v-if="!returnHome">
              <!-- Submitting the form -->
              <div class="text-center">
                <v-progress-circular
                  indeterminate
                  color="primary"
                  id="progress"
                  :size="50"
                ></v-progress-circular>
                <p class="text--disabled">Submitting form</p>
              </div>
            </div>

            <div v-else>
              <!-- Display submission status -->
              <div v-if="submissionStatus">
                <v-card-title class="headline text-center" id="submited">
                  Your form has been submitted!
                </v-card-title>

                <v-card-text class="text-center" id="submissionError">
                  Some text on what will come next for the employee.
                </v-card-text>
              </div>
              <div v-else>
                <v-card-title class="headline" id="failure">
                  Something has gone wrong
                </v-card-title>

                <v-card-text>
                  Please try again.
                </v-card-text>
              </div>
            </div>
          </div>
        </v-card>
      </div>

      <!-- The form is not valid -->
      <div v-else>
        <v-card>
          <v-card-title class="headline text-danger" id="invalid">
            Your form is not valid.
          </v-card-title>

          <v-card-text>
            Please fix the invalid parts of the form and then retry submitting
            your form.
            <hr />
            Errors:
            <v-card
              color="red lighten-2"
              v-for="(error, index) in errors"
              :key="index"
            >
              <strong>{{ error }}</strong>
            </v-card>
          </v-card-text>
        </v-card>
      </div>
    </v-dialog>
  </v-container>
</template>

<style>
  .v-card__text,
  .v-card__title {
    word-break: normal; /* maybe !important  */
  }
  .v-progress-circular {
    margin: 1rem;
  }
  .p {
    margin-bottom: 0 !important;
  }
  .v-application p {
    margin-bottom: 0 !important;
  }
</style>

<script>
  import axios from "axios";
  import store from "@/store/index.js";
  import { mapFields } from "vuex-map-fields";
  import { FORM, FORM_TYPE } from "@/components/Utility/Enums.js";

  export default {
    name: "ConfirmSubmission",
    props: {
      // The cols in the datatable
      cols: {
        type: Array,
        default: null,
      },

      //If the information is valid.
      valid: {
        type: Boolean,
        default: false,
      },

      // Signal that parent form has completed validation
      validationSignal: {
        type: Boolean,
        default: false,
      },

      // The list of errors from the parent's validation function
      errors: {
        type: Array,
        default: null,
      },

      // The amount of errors from the parent's validation function
      numErrors: {
        type: Number,
        default: 0,
      },
    },

    data() {
      return {
        // Hide or display the submit pop-up
        displaySubmit: false,

        //Log of POST connection.
        loading: false,

        //Track when the POST completes
        submissionStatus: false,

        //Flag for once POST has been successful/failed
        returnHome: false,

        //Data to be submitted
        submitData: null,

        // All the errors of this form
        isValid: this.valid,
        waitingOnParent: false,

        // Provider and employer re-signed the form
        reSigned: [],
      };
    },

    computed: {
      url: function () {
        //URL for the AppServer
        if (FORM[this.formChoice] === FORM.OR004_MILEAGE)
          return process.env.VUE_APP_SERVER_URL.concat("Timesheet/SubmitMileage");
        else
          return process.env.VUE_APP_SERVER_URL.concat("Timesheet/Submit");
      },
      canSubmit: function () {
        return (
          this.totalEdited > 0 && !(this.reSigned.length === 2) && this.isValid
        );
      },
      ...mapFields(["formChoice", "formId", "onlineStatus"]),
      formType: function () {
        return FORM_TYPE[this.formChoice];
      },
      totalEdited: function () {
        if (this.formType !== undefined && store.getters !== undefined)
          return store.getters[this.formType + "/getField"]("totalEdited");
        else return 0;
      },
      formFields: function () {
        if (this.formType !== undefined && store.getters !== undefined)
          return store.getters[this.formType + "/getField"]("formFields");
        else return null;
      },
    },

    watch: {
      // The parent form has finished validating all fields on the
      // IDD Timesheet. Display errors or submit
      validationSignal() {
        var numErrors = this.errors.length;
        if (numErrors > 0) {
          this.isValid = false;
        } else {
          this.isValid = true;
        }

        if (this.waitingOnParent === true) {
          this.waitingOnParent = false;
          this.displaySubmit = true;
        }
      },
    },

    methods: {
      // Reset all submission values
      resetValid() {
        this.reSigned = [];
        this.loading = false;
        this.submissionStatus = false;
        this.returnHome = false;
      },

      //Formats the data to be posted
      formatData() {
        var submitData = {};
        Object.entries(this.formFields).forEach(([key, value]) => {
          submitData[key] = {};
          submitData[key]["value"] = value["value"];
          submitData[key]["wasEdited"] = !value["disabled"];
        });
        var sheetType = "";
        if ("timesheet" in this.formFields) {
          sheetType = "timesheet";
        } else if ("mileagesheet" in this.formFields) {
          sheetType = "mileagesheet";
        }
        if (sheetType.localeCompare("") !== true) {
          submitData[sheetType]["value"] = [];
          Object.entries(this.formFields[sheetType]["value"]).forEach(
            ([key, value]) => {
              key;
              var row = {};

              this.cols.forEach((col) => {
                row[col] = value[col];
              });
              row["wasEdited"] = !value["disabled"];

              submitData[sheetType]["value"].push(row);
            }
          );
        }

        return submitData;
      },

      // Send signal to parent component to validate
      signalParentValidate() {
        // Reset the submission values
        this.resetValid();

        // Set flag to wait on parent
        this.waitingOnParent = true;

        // Send signal to parent component to validate input fields
        this.$emit("click");
      },

      //Submits form to AppServer.
      submit() {
        // Do not post timesheet if any:
        //   - The form is invalid
        //   - There were edits, but employer and provider didn't re-sign
        if (this.canSubmit) {
          return false;
        }

        // Else, post timesheet
        this.loading = true;
        let self = this;

        // Finally, prepare the form data and send to the backend
        this.submitData = this.formatData();
        if (this.errors.length === 0) {
          this.submitData["id"] = this.formId;
          this.submitData["formChoice"] = FORM[this.formChoice];
          axios
            .post(this.url, this.submitData, {
              headers: {
                "content-type": "application/json",
              },
            })
            .then(function (response) {
              if (response["data"]["response"] == "ok") {
                console.log("Finished posting!");
                self.submissionStatus = true;

                //Return to home here?
                self.returnHome = true;
              }
            })
            .catch(function (error) {
              console.log(error);
            });
        }
      },
    },
  };
</script>
