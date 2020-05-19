<template>
  <v-container :fill-height="askContinue" fluid>
    <!-- If there is already parsed form data, ask if the user wants to continue -->
    <template v-if="askContinue">
      <v-row align="center" justify="center">
        <v-col cols="12" md="4" sm="8">
          <v-card>
            <v-card-title>
              Continue existing form?
            </v-card-title>
            <v-card-text>
              Form already exists! You are working on form id#{{ formId }}<br />
              Do you want to continue or start new? <br />
            </v-card-text>
            <v-card-actions>
              <v-btn class="white--text" color="red" @click="resetForm()">
                reset
              </v-btn>
              <v-btn
                class="white--text"
                color="green"
                @click="willContinue = true"
              >
                continue
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </template>
    <template v-else>
      <!-- Have the user choose which form they want to upload -->
      <v-row class="mt-9 mx-9">
        <v-col align="center">
          <p class="title">
            Select the type of form that you would like to submit:
          </p>
          <v-select
            :items="Object.keys(FORM)"
            label="Timesheet"
            v-model="formChoice"
            outlined
          >
            <v-btn
              icon
              color="red"
              class="white--text"
              v-if="newForm === false"
              slot="prepend"
              @click="resetForm()"
            >
              <v-icon>mdi-delete</v-icon>
            </v-btn>
          </v-select>
        </v-col>
      </v-row>

      <!-- Page Title -->
      <v-row class="mt-9">
        <v-col align="center">
          <p class="headline">
            {{ formChoice ? formChoice : "Please select a form type!" }} <br />
          </p>
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
import mockData from "@/components/Utility/happy_path.json";
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
        parsedFileData: mockData,

        // Possible statuses of the uploading the form
        fileStatus: FILE.SUCCESS,

        // Upload errors
        errors: [],

        // Will continue editing an existing timesheet or no
        willContinue: false,
      };
    },
    computed: {
      ...mapFields(["formId", "formChoice", "newForm"]),
      askContinue() {
        return this.newForm === false && this.willContinue === false;
      },
    },
    methods: {
      ...mapMutations(["resetState"]),

      // Successfully received parsed .json from the backend
      fillForm(response) {
        // Save the parsed .json
        this.parsedFileData = response;

        // Hide the image upload and display the pre-populated IDD form
        this.fileStatus = FILE.SUCCESS;
        this.willContinue = true;
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
      },
    },
  };
</script>
