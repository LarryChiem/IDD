<template>
  <div>
    <!-- Page Title -->
    <v-row class="mt-9">
      <v-col align="center" justify="center">
        <p class="headline">
          eXPRS Plan of Care - Services Delivered Report Form
        </p>
      </v-col>
    </v-row>

    <!-- Render either file upload or form -->
    <v-row>
      <v-col v-if="fileStatus === 1 || fileStatus === 3">
        <FileUploader
          @error="handleError($event)"
          @success="fillForm($event)"
        />
        <v-card v-if="fileStatus === 3" class="ma-5">
          <v-card-title class="error white--text"
            >FILE UPLOAD ERROR!</v-card-title
          >
          <v-card-text>
            {{ errors }}
          </v-card-text>
        </v-card>
      </v-col>

      <v-col v-else-if="fileStatus === 2">
        <IDDForm :parsedFileData="parsedFileData" />
      </v-col>
    </v-row>
  </div>
</template>

<script>
  import FileUploader from "@/components/Timesheet/FileUploader";
  import IDDForm from "@/components/Timesheet/IDDForm";

  export default {
    name: "Timesheet",
    components: {
      FileUploader,
      IDDForm,
    },
    data: function () {
      return {
        // The uploaded timesheet, as a .json of parsed values from the backend
        parsedFileData: null,

        // Possible statuses of the uploading the form:
        //   - 1 form not uploaded
        //   - 2 form successfully uploaded
        //   - 3 form unsuccessfully uploaded
        // Props isn't going to work unless you define it in a diff file
        fileStatus: 1,

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
        this.fileStatus = 2;
      },
      handleError(error) {
        this.errors = error;
        this.fileStatus = 3;
      },
    },
  };
</script>
