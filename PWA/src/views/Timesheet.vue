<template>
  <div>
    <!-- Have the user choose which form they want to upload -->
    <v-row class="mt-9">
      <v-col align="center" justify="center">
        Select the type of form that you would like to submit:
        <v-select
          :items="Object.keys(FORM)"
          label="Timesheet"
          v-model="formChoice"
          outlined
        ></v-select>
      </v-col>
    </v-row>

    <!-- Page Title -->
    <v-row class="mt-9">
      <v-col align="center" justify="center">
        <p class="headline">
          {{ formChoice ? formChoice : "Please select a form type!" }} <br />
        </p>
      </v-col>
    </v-row>

    <!-- Render either file upload or form -->
    <v-row v-if="formChoice !== null">
      <v-col v-if="fileStatus === FILE.INIT || fileStatus === FILE.FAILURE">
        <FileUploader
          :isOnline="isOnline"
          @error="handleError($event)"
          @success="fillForm($event)"
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
          :isOnline="isOnline"
        />
      </v-col>

      <v-col
        v-else-if="
          fileStatus === FILE.SUCCESS && FORM[formChoice] === FORM.OR004_MILEAGE
        "
      >
        <Mileage
          :parsedFileData="parsedFileData"
          :formChoice="FORM[formChoice]"
          :isOnline="isOnline"
        />
      </v-col>
    </v-row>
  </div>
</template>

<script>
  import FileUploader from "@/components/Forms/FileUploader";
  import ServicesDelivered from "@/components/Forms/ServicesDelivered/ServicesDelivered";
  import Mileage from "@/components/Forms/Mileage/Mileage";
  import { FORM, FILE } from "@/components/Utility/Enums.js";

  export default {
    name: "Timesheet",
    components: {
      FileUploader,
      ServicesDelivered,
      Mileage,
    },
    props: {
      isOnline: {
        type: Boolean,
        default: false,
      },
    },
    data: function () {
      return {
        // Expose the imported enums to the html section
        FILE: FILE,
        FORM: FORM,

        // The uploaded timesheet, as a .json of parsed values from the backend
        parsedFileData: null,
        formChoice: null,

        // Possible statuses of the uploading the form
        fileStatus: FILE.INIT,

        // Upload errors
        errors: [],
      };
    },
    methods: {
      // Successfully received parsed .json from the backend
      fillForm(response) {
        // Save the parsed .json
        this.parsedFileData = response;

        // Hide the image upload and display the pre-populated IDD form
        this.fileStatus = FILE.SUCCESS;
      },
      handleError(error) {
        this.errors = error;
        this.fileStatus = FILE.FAILURE;
      },
    },
  };
</script>
