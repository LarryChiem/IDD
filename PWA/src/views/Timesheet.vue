<template>
  <!-- Prompt Upon Detecting Form Draft -->
  <v-container :fill-height="askContinue" :class="continueColor" fluid>
    <!-- If there is already parsed form data, ask if the user wants to continue -->
    <template v-if="askContinue">
      <v-row align="center" justify="center">
        <v-col cols="12" md="6" sm="8">
          <v-dialog value="true" hide-overlay persistent>
            <v-card>
              <v-card-title class="indigo white--text">
                {{ $t("views_Timesheet_continue") }}
              </v-card-title>
              <v-card-text class="text-center subtitle-1 mt-3">
                {{ $t("views_Timesheet_continue_desc0") }}
                <br />
                {{ $t("views_Timesheet_continue_desc1") }}
              </v-card-text>

              <v-divider></v-divider>

              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn class="white--text" color="red" @click="resetForm()">
                  {{ $t("views_Timesheet_continue_btn0") }}
                </v-btn>
                <v-btn
                  class="white--text"
                  color="green"
                  @click="setWillContinue()"
                >
                  {{ $t("views_Timesheet_continue_btn1") }}
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-col>
      </v-row>
    </template>
    <!-- END Prompt Upon Detecting Form Draft -->
    
    <template v-else>
      <!-- Form Type Selector -->
      <v-row class="mt-9 mx-9">
        <v-col align="center">
          <p class="title">
            {{ $t("views_Timesheet_select") }}
          </p>
        </v-col>
      </v-row>
      <v-row class="mt-9 mx-9">
        <v-col cols="1" v-if="newForm === false">
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
            :label="$t('views_Timesheet_timesheet')"
            v-model="formChoice"
            outlined
          >
          </v-select>
        </v-col>
      </v-row>
      <!-- END Form Type Selector -->
      
      <!-- Invalid Form Uploaded Notification -->
      <v-row v-if="invalidForm === true" align="center">
        <v-col align="center">
          <v-alert border="left" type="warning" text outlined>
            {{ $t("views_Timesheet_invalid") }}
          </v-alert>
        </v-col>
      </v-row>
      <v-row v-else-if="blurryForm === true" align="center">
        <v-col align="center">
          <v-alert border="left" type="warning" text outlined>
            {{ $t("views_Timesheet_blurry") }}
          </v-alert>
        </v-col>
      </v-row>
      <!-- END Invalid Form Uploaded Notification -->
      
      <v-divider />

      <!-- Page Title -->
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
            color="warning"
            text
            outlined
            v-else
          >
            {{ $t("views_Timesheet_select_form") }}
          </v-alert>
        </v-col>
      </v-row> 
      <!-- END Page Title -->

      <!-- Render either file upload section or form section -->
      <v-row v-if="formChoice !== null">
        <!-- Upload New Form Section -->
        <v-col v-if="fileStatus === FILE.INIT || fileStatus === FILE.FAILURE">
          <FileUploader
            @error="handleError($event)"
            @success="fillForm($event)"
            @reset="fileStatus = FILE.INIT"
          />
          
          <!-- File Upload Error Section -->
          <v-card v-if="fileStatus === FILE.FAILURE" class="ma-5">
            <v-card-title class="error white--text">
              {{ $t("views_Timesheet_upload_error") }}
            </v-card-title>
            <v-card-text>
              {{ errors }}
            </v-card-text>
          </v-card>
          <!-- END File Upload Error Section -->

        </v-col>
        <!-- END Upload New Form Section -->
        
        <!-- Form Section -->
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
        <!-- END Form Section -->

      </v-row>
    </template>
  </v-container>
</template>

<script>
  import FileUploader from "@/components/Forms/FileUploader";
  import Mileage from "@/components/Forms/Mileage/Mileage";
  import ServicesDelivered from "@/components/Forms/ServicesDelivered/ServicesDelivered";
  import { mapFields } from "vuex-map-fields";
  import { mapMutations } from "vuex";
  import { FORM, FILE } from "@/components/Utility/Enums.js";
  
  export default {
    name: "Timesheet",
    components: {
      FileUploader,
      Mileage,
      ServicesDelivered,
    },
    data: function () {
      return {
        // Expose the imported enums to the html section
        FILE: FILE,
        FORM: FORM,

        // The uploaded timesheet, as a .json of parsed values from the backend
        parsedFileData: null,

        // Possible statuses of the uploading the form
        fileStatus: FILE.INIT,

        // Upload errors
        errors: [],
        blurryForm: false,

        // Will continue editing an existing timesheet or no
        willContinue: false,
      };
    },
    computed: {
      // Import relevant fields in the vuex store
      ...mapFields(["formId", "formChoice", "newForm", "invalidForm"]),
      
      // Check if there is already a parsed form in the cache
      askContinue() {
        return this.newForm === false && this.willContinue === false;
      },
      
      // Darken background when asking user if they will continue editing 
      // a form draft
      continueColor() {
        return this.askContinue ? "grey darken-1" : "";
      },
    },
    methods: {
      // Import the functions for manipulating the vuex store
      ...mapMutations({
        resetState: "resetState",
        resetServiceDelivered: "ServiceDelivered/resetState",
        resetMileage: "Mileage/resetState",
      }),

      // Successfully received parsed .json from the backend
      fillForm(response) {
        this.blurryForm = false;
        if (response.response === "too blurry") {
          this.resetForm();
          this.blurryForm = true;
        } else {
          // Check if textract had trouble parsing the form
          if (response.response === "invalid") {
            this.invalidForm = true;
          }

          // Save the parsed .json
          this.parsedFileData = response;

          // Hide the image upload and display the pre-populated IDD form
          this.setWillContinue();
        }
      },
      
      // Display the error prompt
      handleError(error) {
        this.errors = error;
        this.fileStatus = FILE.FAILURE;
      },

      // Reset the vuex store
      resetForm() {
        this.resetState();
        this.resetServiceDelivered();
        this.resetMileage();
        this.parsedFileData = null;
        this.fileStatus = FILE.INIT;
        this.array = [];
        this.willContinue = false;
        this.invalidForm = false;
        this.blurryForm = false;
      },
      
      // There is a form draft, or the Appserver returned with the parsed form text
      // Go to the timesheet/mileagesheet page
      setWillContinue() {
        this.willContinue = true;
        this.fileStatus = FILE.SUCCESS;
        console.log("parsedFileData from Timesheet", this.parsedFileData);
      },
    },
  };
</script>
