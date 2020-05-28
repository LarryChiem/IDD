<template>
  <v-container 
    :fill-height="askContinue" 
    :class="continueColor"
    fluid 
  >
    <!-- If there is already parsed form data, ask if the user wants to continue -->
    <template v-if="askContinue" >
      <v-row align="center" justify="center">
        <v-col cols="12" md="6" sm="8">
          <v-dialog
            value="true"
            hide-overlay
            persistent
            width="50%"
          >
            <v-card>
              <v-card-title class="indigo white--text">
                Continue existing form?
              </v-card-title>
              <v-card-text class="text-center subtitle-1 mt-3">
                Form already exists! You are working on form <strong>id #{{ formId }}</strong><br />
                Do you want to continue or start a new form? <br />
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn class="white--text" color="red" @click="resetForm()">
                  reset
                </v-btn>
                <v-btn
                  class="white--text"
                  color="green"
                  @click="setWillContinue()"
                >
                  continue
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-col>
      </v-row>
    </template>
    <template v-else>
      <!-- Have the user choose which form they want to upload -->
      <v-row class="mt-9 mx-9">
        <v-col align="center">
          <p class="title">
            Select the type of form that you would like to submit
          </p>
        </v-col>
      </v-row>
      <v-row class="mt-9 mx-9">
        <v-col 
          cols="1" 
          v-if="newForm === false"
        >
          <v-btn
            icon
            color="red"
            class="white--text"
            slot="prepend"
            @click="resetForm()"
          >
            <v-icon>mdi-delete</v-icon>
          </v-btn>
        </v-col>
        <v-col>
          <v-select
            :items="Object.keys(FORM)"
            :disabled="!newForm"
            label="Timesheet"
            v-model="formChoice"
            outlined
          >
          </v-select>
        </v-col>
      </v-row>
      
      <!-- Display warning at top if textract can't parse the uploaded imae -->
      <v-row v-if="invalidForm === true" align="center">
        <v-col align="center">
          <v-alert 
            border="left"
            type="warning" 
            text 
            outlined
          >
            Warning: We couldn’t read the text from the file you uploaded. You will have to manually enter all of the form fields.
          </v-alert>
        </v-col>
      </v-row>
      

      <!-- Page Title -->
      <v-divider />
      <v-row class="mt-9">
        <v-col align="center">
          <v-alert 
            class="headline pa-5" 
            color="light-blue"
            text 
            outlined
            v-if="formChoice"
          >
            {{ formChoice }} 
          </v-alert>
          <v-alert 
            class="headline pa-5 mx-9" 
            type="warning"
            text 
            outlined
            v-else
          >
            Please select a form type above.
          </v-alert>
        </v-col>
      </v-row> 

      <!-- Render either file upload or form -->
      <v-row v-if="formChoice !== null">
        <v-col v-if="fileStatus === FILE.INIT || fileStatus === FILE.FAILURE">
          <FileUploader
            @error="handleError($event)"
            @success="fillForm($event)"
            @reset="fileStatus = FILE.INIT"
          />

          <v-card v-if="fileStatus === FILE.FAILURE" class="ma-5">
            <v-card-title class="error white--text"
              >FILE UPLOAD ERROR!</v-card-title
            >
            <v-card-text>
              {{ errors }}
            </v-card-text>
          </v-card>
        </v-col>

        <v-col
          v-else-if="
            fileStatus === FILE.SUCCESS &&
            (FORM[formChoice] === FORM.OR507_RELIEF ||
              FORM[formChoice] === FORM.OR526_ATTENDANT)
          "
        >
          <ServicesDelivered
            :parsedFileData="parsedFileData"
            :formChoice="FORM[formChoice]"
          />
        </v-col>

        <v-col
          v-else-if="
            fileStatus === FILE.SUCCESS &&
            FORM[formChoice] === FORM.OR004_MILEAGE
          "
        >
          <Mileage
            :parsedFileData="parsedFileData"
            :formChoice="FORM[formChoice]"
          />
        </v-col>
      </v-row>
    </template>
  </v-container>
</template>

<script>
  import { mapFields } from "vuex-map-fields";
  import { mapMutations } from "vuex";

  import FileUploader from "@/components/Forms/FileUploader";
  import ServicesDelivered from "@/components/Forms/ServicesDelivered/ServicesDelivered";
  import Mileage from "@/components/Forms/Mileage/Mileage";
  import { FORM, FILE } from "@/components/Utility/Enums.js";
  import mockServiceDelivered from "@/components/Utility/happy_path.json";
  
  export default {
    name: "Timesheet",
    components: {
      FileUploader,
      ServicesDelivered,
      Mileage,
    },
    data: function () {
      return {
        // Expose the imported enums to the html section
        FILE: FILE,
        FORM: FORM,

        // The uploaded timesheet, as a .json of parsed values from the backend
        parsedFileData: process.env.NODE_ENV === 'development'
                        ? mockServiceDelivered
                        : null,

        // Possible statuses of the uploading the form
        fileStatus: process.env.NODE_ENV === 'development'
                    ? FILE.SUCCESS 
                    : FILE.INIT,

        // Upload errors
        errors: [],

        // Will continue editing an existing timesheet or no
        willContinue: false,
      };
    },
    computed: {
      ...mapFields(["formId", "formChoice", "newForm", "invalidForm"]),
      askContinue() {
        return this.newForm === false && this.willContinue === false;
      },
      continueColor() {
        return this.askContinue ? "grey darken-1" : "";
      }
    },
    methods: {
      ...mapMutations(["resetState"]),

      // Successfully received parsed .json from the backend
      fillForm(response) {
        // Save the parsed .json
        this.parsedFileData = response;
        
        // Check if textract had trouble parsing the form
        if (response.response === "invalid") {
          this.invalidForm = true;
        }

        // Hide the image upload and display the pre-populated IDD form
        this.setWillContinue();
      },
      handleError(error) {
        this.errors = error;
        this.fileStatus = FILE.FAILURE;
      },
      resetForm() {
        // Reset the vuex store
        this.resetState();
        this.parsedFileData = null;
        this.fileStatus = FILE.INIT;
        this.array = [];
        this.willContinue = false;
        this.invalidForm = false;
      },
      setWillContinue() {
        this.willContinue = true;
        this.fileStatus = FILE.SUCCESS;
        console.log("parsedFileData from Timesheet:236", this.parsedFileData);
      },
    },
  };
</script>
