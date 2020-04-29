<template>
  <div>
    <br />
    <hr />
    <br />
    <!--
    <span class="md-display-3">Vue Material UI Examples</span>
    <span class="md-headline">Here are some different components from the Vue Material UI library</span>

    <md-toolbar class="md-accent">
      <h3 class="md-title">Form Example</h3>
    </md-toolbar>
    <FormExample />
    
    <br/><hr/><br/>
    
    <md-toolbar class="md-primary">
      <h3 class="md-title">Table Example</h3>
    </md-toolbar>
    <TableExample />

    <br/><hr/><br/>
    
    <md-toolbar>
      <h3 class="md-title">Date Picker Example</h3>
    </md-toolbar>
    <DateExample />

    <br/><hr/><br/>
    
    <md-toolbar>
      <h3 class="md-title">Camera Example</h3>
    </md-toolbar>
    <CameraExample />
    -->
    <md-toolbar>
      <h3 class="md-title">File Upload</h3>
    </md-toolbar>
    <FileUploader />
  </div>
</template>

<script>
  /**import FormExample from './FormExample.vue'
  import TableExample from './TableExample.vue'
  import DateExample from './DateExample.vue'
  import CameraExample from './CameraExample.vue'
  **/
  import FileUploader from "./FileUploader.vue";

  import Vue from "vue";
  import { MdToolbar, MdLayout } from "vue-material/dist/components";
  import "vue-material/dist/vue-material.min.css";
  import "vue-material/dist/theme/default.css";

  Vue.use(MdToolbar);
  Vue.use(MdLayout);

  export default {
    name: "ComponentsExample",
    components: {
      /**FormExample,
      TableExample,
      DateExample,
      CameraExample,
      **/
      FileUploader,
    },
  };
</script>

<style lang="scss" scoped>
  span {
    margin: 0.5em 0;
    display: block;
  }
  .md-toolbar + .md-toolbar {
    margin-top: 16px;
  }
</style>
