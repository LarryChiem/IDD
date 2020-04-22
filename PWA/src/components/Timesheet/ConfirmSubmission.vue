<template>
  <v-row justify="center">
    <v-btn color="success" 
		dark 
		@click.stop="dialog = true" 
		@click="validate">
      Submit
    </v-btn>

    <v-dialog 
		v-model="dialog" 
		max-width="300">
      <div v-if="valid">
        <v-card>
          <div v-if="!loading">
            <v-card-title 
						class="headline" 
						id="confirm"
              >Are you sure want to submit the form?</v-card-title
            >

            <v-card-text>
              Some text talking about how this submission is final unless
              something is wrong with it.
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>

							<!-- Confirm if user is ready to submit -->
              <v-btn color="red" 
							text 
							@click="dialog = false">
                Cancel
              </v-btn>
              <v-btn color="green 
							darken-1" text
							@click="submit">
                Submit
              </v-btn>
            </v-card-actions>
          </div>

          <div v-else>
            <div v-if="!returnHome">
              <!-- Submitting the form -->
              <div class="text-center">
                <v-progress-circular
                  :size="50"
                  color="primary"
                  indeterminate
                  id="progress"
                ></v-progress-circular>
                <p class="text--disabled">Submitting form</p>
              </div>
            </div>

            <div v-else>
              <!-- Display submission status -->
              <div v-if="submissionStatus">
                <v-card-title 
								class="headline 
								text-center" 
								id="submited"
                  >Your form has been submitted!</v-card-title
                >

                <v-card-text class="text-center" id="submissionError">
                  Some text on what will come next for the employee.
                </v-card-text>
              </div>
              <div v-else>
                <v-card-title 
								class="headline" 
								id="failure"
                  >Something has gone wrong</v-card-title
                >

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
					<v-card-title 
					class="headline 
					text-danger" 
					id="invalid">
						Your form is not valid.</v-card-title
					>
	
					<v-card-text>
						Please fix the invalid parts of the form and then retry submitting
						your form.
					</v-card-text>
				</v-card>
      </div>
    </v-dialog>
  </v-row>
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

  export default {
    name: "ConfirmSubmission",
    props: {
      //If the information is valid.
      valid: {
        type: Boolean,
        default: false,
      },

      //User (edited) information.
      formFields: {
        type: Object,
        default: null,
      },
    },

    data() {
      return {
        //If the dialog is still up.
        dialog: false,

        //Log of POST connection.
        loading: false,

        //Track when the POST completes
        submissionStatus: false,

        //Flag for once POST has been successful/failed
        returnHome: false,

        //Data to be submitted
        submitData: null,

        //URL for the AppServer
        url: process.env.VUE_APP_SERVER_URL.concat("Submit"),
      };
    },

    methods: {
      validate() {
        if (this.valid) {
          console.log("Valid form");
        }
      },

      //Formats the data to be posted
      formatData() {
        var submitData = {};
        Object.entries(this.formFields).forEach(([key, value]) => {
          submitData[key] = {};
          submitData[key]["value"] = value["value"];
          submitData[key]["wasEdited"] = !value["disabled"];
        });
        submitData["serviceDeliveredOn"]["value"] = [];
        submitData["serviceDeliveredOn"]["wasEdited"] = false; // TODO
        Object.entries(this.formFields["serviceDeliveredOn"]["value"]).forEach(
          ([key, value]) => {
            key;
            var row = {};
            var cols = ["date", "startTime", "endTime", "totalHours", "group"];
            cols.forEach((col) => {
              var c = {};
              c["value"] = value[col]; // TODO value[col]['value']
              c["wasEdited"] = false; // TODO
              row[col] = c;
            });
            submitData["serviceDeliveredOn"]["value"].push(row);
          }
        );
        this.submitData = submitData;
      },

      //Submits form to AppServer.
      submit() {
        //After form is validated, post timesheet.
        this.loading = true;
        var self = this;
        //Prepare the data to send.
        this.formatData();

        if (this.valid) {
          axios
            .post(this.url, this.submitData, {
              headers: {
                "content-type": "text/plain",
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
