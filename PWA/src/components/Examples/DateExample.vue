<template>
  <div>
    <md-datepicker v-model="selectedDate">
      <label>Select date</label>
    </md-datepicker>
  </div>
</template>

<script>
  import Vue from "vue";
  import {
    MdDatepicker,
    MdDialog,
    MdLayout,
  } from "vue-material/dist/components";
  import "vue-material/dist/vue-material.min.css";
  import "vue-material/dist/theme/default.css";

  Vue.use(MdDatepicker);
  Vue.use(MdDialog);
  Vue.use(MdLayout);

  export default {
    name: "DateExample",
    data: () => ({
      selectedDate: null,
    }),
  };
</script>
