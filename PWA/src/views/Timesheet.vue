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

    <!-- Select to toggle between file upload submition and form submition -->
    <v-row>
      <v-col class="mx-9">
        <v-select
          clearable
          label="Timesheet submission method:"
          light
          v-model="variant"
          :items="items"
        ></v-select>
      </v-col>
    </v-row>

    <v-divider></v-divider>

    <!-- Render either file upload or form -->
    <v-row>
      <v-col v-if="variant === 'Upload an image'">
        <FileUploader />
      </v-col>

      <v-col v-else-if="variant === 'Fill out a form'">
        <IDDForm :parsedFileData="parsedFileData" />
      </v-col>
    </v-row>
  </div>
</template>

<script>
  import FileUploader from "@/components/Timesheet/FileUploader";
  import IDDForm from "@/components/Timesheet/IDDForm";

  // MOCK .json data
  import mock_json from "@/components/Timesheet/OR507_Example.json";

  export default {
    name: "Timesheet",
    components: {
      FileUploader,
      IDDForm,
    },
    data: () => ({
      // Choices for the select component
      items: ["Upload an image", "Fill out a form"],
      variant: "default",
      parsedFileData: mock_json,
    }),
  };
</script>
