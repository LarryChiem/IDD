<template>
  <v-expansion-panels color="primary" :accordion="true">
    <v-expansion-panel>
      <v-expansion-panel-header class="body-1 py-0 my-0">
        <v-container>
          <v-row no-gutters>
            <v-col cols="12" md="3">
              <strong>
                Date:
              </strong>
              {{ headers.date.value || "N/A" }}
            </v-col>
            <v-col cols="12" md="3">
              <strong>
                Time:
              </strong>
              {{ headers.startTime.value || "N/A" }} -
              {{ headers.endTime.value || "N/A" }}
            </v-col>
            <v-col cols="6" md="3">
              <strong>
                Total Hours:
              </strong>
              {{ headers.totalHours.value || "N/A" }}
            </v-col>
            <v-col cols="12" md="3">
              <strong>
                Group:
              </strong>
              {{ headers.group.value ? "Yes" : "No" }}
            </v-col>
          </v-row>
        </v-container>
      </v-expansion-panel-header>

      <v-expansion-panel-content>
        <v-container justify="space-around" no-gutters>
          <v-row
            class="px-2"
            v-for="field in [
              'date',
              'startTime',
              'endTime',
              'totalHours',
              'group',
            ]"
            :key="field"
          >
            <FormField v-bind="headers[field]" v-model="headers[field].value" />
          </v-row>
        </v-container>
      </v-expansion-panel-content>
    </v-expansion-panel>
  </v-expansion-panels>
</template>

<script>
  import FormField from "@/components/Form/FormField";
  import rules from "@/components/Utility/FormRules.js";

  export default {
    name: "FormTable",

    components: {
      FormField,
    },

    data: function () {
      return {
        headers: {
          date: {
            disabled: false,
            hint: "YYYY-MM-DD",
            label: "Date",
            rules: rules["dateRules"],
            value: null,
          },
          startTime: {
            counter: 8,
            disabled: false,
            hint: "HH:mm AM/PM",
            label: "Start/Time IN",
            rules: rules["timeRules"],
            value: "",
          },
          endTime: {
            counter: 8,
            disabled: false,
            hint: "HH:mm AM/PM",
            label: "End/Time OUT",
            rules: rules["timeRules"],
            value: "",
          },
          totalHours: {
            counter: 8,
            disabled: false,
            hint: "HH:mm AM/PM",
            label: "Total Hours",
            rules: rules["timeRules"],
            value: "",
          },
          group: {
            disabled: false,
            field_type: 1,
            label: "Group?",
            value: false,
          },
        },
      };
    },
  };
</script>
