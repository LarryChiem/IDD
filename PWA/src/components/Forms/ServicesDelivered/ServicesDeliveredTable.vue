<template>
  <v-data-table class="elevation-1" :headers="headers" :items="allEntries">
    <!-- The header of the table -->
    <template v-slot:top>
      <v-toolbar flat>
        <v-toolbar-title>
          {{
            $t(
              "components_Forms_ServicesDelivered_ServicesDeliveredTable_title"
            )
          }}
        </v-toolbar-title>

        <v-spacer></v-spacer>

        <!-- Warning dialog upon editing a parsed field -->
        <v-dialog max-width="500px" v-model="displayWarning">
          <v-card>
            <v-card-title class="headline">
              {{ $t("components_Forms_FormField_edit") }}
            </v-card-title>

            <v-card-text>
              {{ $t("components_Forms_FormField_edit_desc") }}
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>

              <v-btn color="red white--text" @click="closeWarning()">
                {{ $t("components_Forms_FormField_cancel") }}
              </v-btn>

              <v-btn color="green white--text" @click="warnContinue()">
                {{ $t("components_Forms_FormField_editbtn") }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- END warning dialog upon editing a parsed field -->

        <!-- Appears upon adding/editing an item -->
        <v-dialog max-width="500px" v-model="displayEditDialog">
          <!-- Button group, visible even though dialogue is hidden -->
          <template class="d-flex flex-row-reverse" v-slot:activator="{ on }">
            <v-btn-toggle dense multiple>
              <!-- Lock/unlock addding a row to the table -->
              <v-btn @click="askTableEdit($event)">
                <v-icon color="primary" v-if="amtEdited < 1">mdi-lock</v-icon>
                <v-icon v-else>refresh</v-icon>
              </v-btn>

              <!-- Add a row button -->
              <v-btn
                color="success"
                v-on="on"
                :disabled="amtEdited < 1"
                @click="
                  editingTable = true;
                  focusedElement = $event.target;
                "
              >
                <v-icon color="white">mdi-plus</v-icon>
              </v-btn>

              <!-- Reset table button -->
              <v-btn
                color="error"
                :disabled="amtEdited < 1"
                @click="
                  focusedElement = $event.target;
                  initialize();
                "
              >
                <v-icon color="white">mdi-refresh</v-icon>
              </v-btn>
            </v-btn-toggle>
          </template>

          <!-- The dialog box title -->
          <v-card>
            <v-card-title>
              <span class="headline">{{
                $t(
                  "components_Forms_ServicesDelivered_ServicesDeliveredTable_table_title"
                )
              }}</span>
            </v-card-title>

            <!-- The form area -->
            <v-card-text v-if="displayEditDialog">
              <v-container>
                <v-row
                  class="py-0 my-0"
                  v-for="field in ['date', 'starttime', 'endtime']"
                  :key="field"
                >
                  <FormField
                    v-bind="colValidation[field]"
                    v-model="editedItem[field]"
                    :label="$t(colValidation[field].label)"
                    :hint="$t(colValidation[field].hint)"
                  />
                </v-row>

                <v-row class="py-0 my-0">
                  {{ $t("components_Forms_ServicesDelivered_totalhours") }}
                  {{ editedItemTotalHours }}
                </v-row>

                <v-checkbox
                  :label="
                    $t(
                      'components_Forms_ServicesDelivered_ServicesDeliveredTable_group'
                    )
                  "
                  true-value="1"
                  false-value="0"
                  :input-value="editedItem.group"
                  @change="flipGroup(editedItem)"
                  @keyup.native.enter.stop="flipGroup(editedItem)"
                  :disabled="!displayEditDialog"
                ></v-checkbox>
              </v-container>
            </v-card-text>
            <!-- END form area -->

            <!-- Cancel/Save edited item panel -->
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="red white--text" @click="close">
                {{ $t("components_Forms_FormField_cancel") }}
              </v-btn>
              <v-btn color="green white--text" @click="save">
                {{
                  $t(
                    "components_Forms_ServicesDelivered_ServicesDeliveredTable_save"
                  )
                }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- END Appears upon adding/editing an item -->
      </v-toolbar>
    </template>
    <!-- END table toolbar -->

    <!-- Table content defined in props of v-data-table -->

    <!-- Layout specification for cols that can be invalid -->
    <template v-slot:item.date="{ item }">
      <v-container flat :class="getColor(item.errors, 'date')">
        {{ item.date }}
      </v-container>
    </template>

    <template v-slot:item.starttime="{ item }">
      <v-container flat :class="getColor(item.errors, 'starttime')">
        {{ item.starttime }}
      </v-container>
    </template>

    <template v-slot:item.endtime="{ item }">
      <v-container flat :class="getColor(item.errors, 'endtime')">
        {{ item.endtime }}
      </v-container>
    </template>

    <template v-slot:item.totalHours="{ item }">
      <v-container flat :class="getColor(item.errors, 'totalHours')">
        {{ item.totalHours }}
      </v-container>
    </template>

    <template v-slot:item.group="{ item }">
      <v-container flat :class="getColor(item.errors, 'group')">
        {{ item.group == "1" ? $t("yes") : $t("no") }}
      </v-container>
    </template>

    <!-- The action column of the table -->
    <template v-slot:item.actions="{ item }">
      <v-container class="d-flex align-center pa-0">
        <!-- Unlock editing for a row of the table -->
        <!-- Rather than true/false values, use "Yes"/"No" -->
        <v-checkbox
          class="ma-0 ma-0"
          color="primary"
          hide-details
          off-icon="refresh"
          on-icon="lock"
          tabindex="0"
          v-if="item.parsed === true"
          v-model="item.disabled"
          @click.stop="warnEdit($event, item)"
          @keyup.native.enter.stop="warnEdit($event, item)"
        ></v-checkbox>

        <!-- Edit a row of the table -->
        <v-icon :disabled="item.disabled" @click="editItem($event, item)">
          mdi-pencil
        </v-icon>

        <!-- Delete a row of the table -->
        <v-icon :disabled="item.disabled" @click="deleteItem(item)">
          mdi-delete
        </v-icon>
      </v-container>
    </template>
  </v-data-table>
</template>

<script>
  import FormField from "@/components/Forms/FormField";
  import fieldPropsFile from "@/components/Forms/ServicesDelivered/ServicesDeliveredTableFields.json";
  import i18n from '@/plugins/i18n';
  import rules from "@/components/Utility/FormRules.js";
  import { TIME } from "@/components/Utility/Enums.js";
  import {
    subtractTime,
    milliToFormat,
    isValid,
  } from "@/components/Utility/TimeFunctions.js";
  
  var moment = require("moment");
  
  import { mapFields } from "vuex-map-fields";

  export default {
    name: "ServicesDeliveredTable",
    components: {
      FormField,
    },
    props: {
      // A .json file that is a section from the parsed uploaded IDD timesheet data
      value: {
        type: Array,
        default: null,
      },
      // The columns of this table
      cols: {
        type: Array,
        default: null,
      },
      // Is adding a row enabled?
      disabled: {
        type: Boolean,
        default: false,
      },
      // Was this table edited anywhere?
      modified: {
        type: Boolean,
        default: true,
      },
      // Does this table have parsed values?
      parsed: {
        type: Boolean,
        default: false,
      },
      // The parsed values for this table (used for resetting)
      parsed_value: {
        type: Array,
        default: null,
      },
      // Reset to default props or no?
      reset: {
        type: Boolean,
        default: false,
      },
      // Sum of all the totalHours for each row
      totalHours: {
        type: String,
        default: "00:00",
      },
      // Do we have to display the 'edit warning' prompt?
      willResign: {
        type: Boolean,
        default: false
      },
      // Display or hide the 'edit warning' prompt
      editTable: {
        type: Boolean,
        default: false
      },
      // How many fields were edited/added/deleted
      amountEdited: {
        type: Number,
        default: 0 
      },
    },

    data: function() {
      return {
        // Specify rules and hints for adding a new row to the table
        colValidation: JSON.parse(
          JSON.stringify(fieldPropsFile["colValidation"])
        ),

        // Record the amount of edited parsed fields and added rows
        amtEdited: this.amountEdited,
        allTotalHours: this.totalHours,

        // Hide the warning popup for unlocking a parsed row or adding a row
        editingTable: this.editTable,
        displayWarning: false,

        // Hide the dialog popup for adding/editing a row
        displayEditDialog: false,

        // All entries of the data table
        allEntries: [],

        // Index of the row that is currently being added/edited
        editedIndex: -1,

        // Default values for a new row in the table
        defaultItem: {
          date: "",
          starttime: "",
          endtime: "",
          totalHours: "",
          group: "0",
          disabled: false,
          parsed: false,
          errors: {},
        },

        // Helper object for holding changes to a row in the table before
        // saving changes to that row
        editedItem: {
          date: "",
          starttime: "",
          endtime: "",
          totalHours: "",
          group: "0",
          disabled: false,
          parsed: false,
          errors: {},
        },

        // The last focused element before a dialog/popup appears
        // This allows for resuming tabbing after the dialog/popup closes
        focusedElement: null,
      };
    },

    computed: {
      // Column headers and associated values for the table
      headers: function () {
        return [
          {
            text: i18n.t("ServicesDeliveredTable_date_label"),
            align: "start",
            value: "date",
            sortable: false,
          },
          {
            text: i18n.t("ServicesDeliveredTable_starttime_label"),
            value: "starttime",
            sortable: false,
          },
          {
            text: i18n.t("ServicesDeliveredTable_endtime_label"),
            value: "endtime",
            sortable: false,
          },
          {
            text: i18n.t("ServicesDelivered_totalHours_label"),
            value: "totalHours",
            sortable: false,
          },
          {
            text: i18n.t(
              "components_Forms_ServicesDelivered_ServicesDeliveredTable_group"
            ),
            value: "group",
            sortable: false,
          },
          {
            text: i18n.t("MileageTable_actions"),
            value: "actions",
            sortable: false,
          },
        ];
      },
      editedItemTotalHours: function () {
        var start = this.editedItem["starttime"];
        var end = this.editedItem["endtime"];
        var timeDiff = subtractTime(start, end, TIME.TIME_12);
        var formatTimeDiff = milliToFormat(timeDiff, TIME.TIME_24);
        this.$set(this.editedItem, "totalHours", formatTimeDiff);
        return formatTimeDiff;
      },
      
      ...mapFields(["newForm"]),
    },

    watch: {
      // Reset table if parent component sends a reset signal
      reset() {
        this.initialize();
        this.validate();
      },
    },

    created: function () {
      // Bind validation rules to each field that has a 'rules' string
      // specified
      Object.entries(this.colValidation).forEach(([key, value]) => {
        if ("rules" in value) {
          const _rules = value.rules;
          let _transRules = [];
          _rules.forEach((fieldRule) => {
            if (typeof fieldRule === "string") {
              _transRules.push(rules[fieldRule]());
              this.colValidation[key].rules.push(rules[fieldRule]());
            }
          });

          if (this.colValidation[key].counter) {
            _transRules.push(rules.maxLength(this.colValidation[key].counter));
          }

          this.$set(this.colValidation[key], "rules", _transRules);
        }
      });
      if (this.newForm === true) {
        this.initialize();
        this.validate();
      } else {
        this.rebind();
        this.validate();
      }
    },

    methods: {
      // Parse .json from props into rows for the data table
      initialize() {
        // Reset the entries in the table & notify parent of change
        this.allEntries = [];
        if (this.amtEdited > 0) {
          this.$emit("disable-change", this.amtEdited * -1);
        }
        this.amtEdited = 0;
        this.editingTable = false;

        // For each parsed entry from props, create a new table row
        if (this.parsed_value !== null) {
          // For each timesheet table entry, create a new set 'obj'
          this.parsed_value.forEach((row) => {
            let obj = {};

            // Only add attributes that fit an existing column header
            Object.entries(row).forEach(([key, value]) => {
              if (key in this.colValidation) {
                this.$set(obj, key, value);
                if (!("parsedValue" in obj)) {
                  this.$set(obj, "parsedValue", {});
                }
                this.$set(obj["parsedValue"], key, value);
              } else {
                console.log(
                  "Unrecognized parsed form field from server: " +
                    `${key} - ${value}`
                );
              }
            });

            // If there was a start and an end time, calculate totalHours
            var start = obj["parsedValue"]["starttime"];
            var end = obj["parsedValue"]["endtime"];
            var totalHours = subtractTime(start, end, TIME.TIME_12);
            if (totalHours > 0) {
              this.$set(
                obj,
                "totalHours",
                milliToFormat(totalHours, TIME.TIME_24)
              );
            }

            // If the parsed row was not empty, add it to the table
            if (Object.keys(obj).length > 0) {
              this.$set(obj, "parsed", true);
              this.$set(obj, "disabled", true);
              this.$set(obj, "errors", {});
              this.allEntries.push(obj);
            }
          });
        }

        // Validate initial table
        this.validate();

        // Update parent
        this.$emit("input", this.allEntries);
      },

      // Add/edit a single row of the table
      editItem(event, item) {
        // Save the row's data into a helper obj
        this.editedIndex = this.allEntries.indexOf(item);
        this.editedItem = Object.assign({}, item);

        // Save the add/edit button to allow for continuation of tabbing
        // after closing the add/edit dialog popup
        this.focusedElement = event.target;

        // Open the add/edit dialog popup
        this.displayEditDialog = true;
      },

      // Flip the true/false value of the 'group' for a given item
      flipGroup(item) {
        if (item.group === "1") {
          item.group = "0";
        } else {
          item.group = "1";
        }
      },

      // Delete a single row of the table
      deleteItem(item) {
        const index = this.allEntries.indexOf(item);
        if (confirm(i18n.t("components_Forms_Mileage_delete"))) {
          this.allEntries.splice(index, 1);
          this.validate();
          this.$emit("input", this.allEntries);
        }
      },

      // Close the add/edit dialog popup
      close() {
        this.displayEditDialog = false;

        // Timeout to wait for dialog to fully close
        setTimeout(() => {
          // Re-initialize the helper object
          this.editedItem = Object.assign({}, this.defaultItem);
          this.editedIndex = -1;

          // resume tabbing to the previously active element
          this.focusedElement.focus();
        }, 200);
      },

      // Close the warning dialog
      closeWarning() {
        this.displayWarning = false;
        setTimeout(() => {
          this.focusedElement.focus();
        }, 200);
      },

      // Save changes to an existing or new row
      save() {
        if (this.editedIndex > -1) {
          Object.assign(this.allEntries[this.editedIndex], this.editedItem);
        } else {
          // Added a new item, so notify parent of change
          this.allEntries.push(this.editedItem);
          this.amtEdited += 1;
          this.$emit("disable-change", 1);
        }
        this.validate();
        this.$emit("input", this.allEntries);
        this.close();
      },

      // If the parsed row is locked, unlock it and notify parent
      // Else, reset row to parsed value and notify parent
      toggleParsed() {
        if (this.editedItem.disabled === true) {
          this.amtEdited += 1;
          this.$emit("disable-change", 1);
        } else {
          // For each field in this row, reset the field to its parsed value
          Object.entries(this.editedItem.parsedValue).forEach(
            ([key, value]) => {
              this.editedItem[key] = value;
            }
          );

          this.amtEdited -= 1;
          this.$emit("disable-change", -1);
        }

        // Flip lock/unlock of parsed row & save
        this.editedItem.disabled = !this.editedItem.disabled;
        this.save();
      },

      // If the add row/reset table buttons are disabled, enable them
      // Else, reset the entire table
      toggleEditTable() {
        if (this.amtEdited > 0) {
          this.initialize();
          this.validate();
        } else {
          this.amtEdited += 1;
          this.$emit("disable-change", 1);
        }
      },

      // Ask before adding a row to the table
      // Or, reset table to the parsed vals
      askTableEdit(event) {
        this.focusedElement = event.target;
        if (this.amtEdited < 1 && this.willResign === false) {
          this.editingTable = true;
          this.displayWarning = true;
        } else {
          this.toggleEditTable();
        }
      },

      // Warn before editing a single parsed table row
      // Or, reset & lock the parsed table row
      warnEdit(event, item) {
        // Save the lock/unlock button to allow for continuation of tabbing
        // after closing the warning
        this.focusedElement = event.target;

        // Select this row in the table and initialize the helper obj
        this.editedIndex = this.allEntries.indexOf(item);
        this.editedItem = Object.assign({}, item);

        // If editting a parsed row, show warning
        // Else, reset the row to its parsed value and lock it
        if (item.disabled === true && this.willResign === false) {
          this.displayWarning = true;
        } else {
          this.toggleParsed();
        }

        // If adding a new row, tell parent of change
        if (this.editingTable === true) {
          this.amtEdited += 1;
          this.$emit("disable-change", 1);
          this.editingTable = false;
        }
      },

      // Runs after pressing the continue button on the warning dialog
      warnContinue() {
        if (this.editingTable === true) {
          this.toggleEditTable();
        } else {
          this.toggleParsed();
        }

        this.closeWarning();
        this.editingTable = false;
      },

      // Validate the table entries
      validate() {
        var amtErrors = 0;
        // The columns to check for validation (ex. exclude action, group)

        // First check that each field has a valid value
        this.allEntries.forEach((entry) => {
          this.cols.forEach((col) => {
            // Reset the list of validation errors for this field for this row
            entry["errors"][col] = [];

            // Run the validation functions associated w/ this field
            if ("rules" in this.colValidation[col]) {
              var wasInvalid = false;
              this.colValidation[col]["rules"].forEach((rule) => {
                // If the validation function fails, add an error to field
                if (rule(entry[col]) !== true) {
                  wasInvalid = true;
                }
              });
              if (wasInvalid === true) {
                entry["errors"][col].push(
                  i18n.t("components_Forms_ServicesDelivered_err0")
                );
              }
            }
          });
        });

        // Next, check that the end time is after the start time
        this.allEntries.forEach((entry) => {
          // If the start and end times are valid, begin parsing
          if (
            entry["errors"]["starttime"].length === 0 &&
            entry["errors"]["endtime"].length === 0
          ) {
            var start = entry["starttime"];
            var end = entry["endtime"];
            var timeDiff = subtractTime(start, end, TIME.TIME_12);
            if (timeDiff <= 0) {
              entry["errors"]["starttime"].push(
                i18n.t("components_Forms_ServicesDelivered_err2")
              );
              entry["errors"]["endtime"].push(
                i18n.t("components_Forms_ServicesDelivered_err2")
              );
            }

            var formatTimeDiff = moment
              .duration({ minutes: timeDiff })
              .format(TIME.TIME_24);
            if (formatTimeDiff.localeCompare(entry["totalHours"]) === 0) {
              entry["errors"]["totalHours"].push(
                i18n.t("components_forms_servicesdelivered_err3")
              );
            }
          }
        });

        // Sort by date & time to check for overlapping time entries
        this.allEntries.sort(
          (start, end) =>
            -1 *
            subtractTime(
              start["date"] + " " + start["starttime"],
              end["date"] + " " + end["starttime"],
              TIME.FULL_DATE
            )
        );
        this.checkOverlapping();

        // Calculate the totalHours
        this.allTotalHours = this.sumTableHours();
        this.$emit("update-totalHours", this.allTotalHours);

        // Count up the amount of errors
        // For each row in the array of entries...
        this.allEntries.forEach((entry, index) => {
          index;
          // For each error col in an entry, check the amount of errors
          Object.entries(entry["errors"]).forEach(([col, errors]) => {
            col;
            if (errors.length > 0) {
              amtErrors += 1;
            }
          });
        });

        return amtErrors;
      },

      checkOverlapping() {
        var ret = 0;
        var prev_end = null;

        this.allEntries.forEach((entry, index) => {
          var start = entry["date"] + " " + entry["starttime"];
          var end = entry["date"] + " " + entry["endtime"];

          if (index !== 0) {
            if (
              isValid(prev_end, TIME.FULL_DATE) &&
              isValid(start, TIME.FULL_DATE)
            ) {
              // If the start is before the end of the prev's end, there is an overlap
              var timeDiff = subtractTime(prev_end, start, TIME.FULL_DATE);
              if (timeDiff <= 0) {
                ret += 1;
                entry["errors"]["starttime"].push(
                  i18n.t("components_Forms_ServicesDelivered_err4")
                );
                entry["errors"]["endtime"].push(
                  i18n.t("components_Forms_ServicesDelivered_err4")
                );
              }
            }
          }

          prev_end = end;
        });

        return ret;
      },

      // Compute the sum of all timesheet totalHours with the totalHours field
      sumTableHours() {
        let totalMilli = 0;

        // For each row in the array of entries...
        this.allEntries.forEach((entry, index) => {
          index;
          if (entry["errors"]["totalHours"].length === 0) {
            totalMilli += moment.duration(entry["totalHours"]).asMilliseconds();
          }
        });

        return milliToFormat(totalMilli, TIME.TIME_24);
      },

      printErrors() {
        this.allEntries.forEach((entry, index) => {
          // For each error col in an entry, check the amount of errors
          Object.entries(entry["errors"]).forEach(([col, errors]) => {
            if (errors.length > 0) {
              console.log(
                i18n.t("components_forms_servicesdelivered_row"),
                index,
                "[",
                col,
                "]: ",
                errors
              );
            }
          });
        });
      },

      // Get the color of a cell in the table
      getColor(errors, field) {
        var ret = "";
        if (field in errors) {
          if (errors[field].length > 0) {
            ret = "red--text red lighten-5 font-weight-bold";
          }
        }
        return ret;
      },

      // Re-bind data to the proper fields from parent's props
      rebind() {
        if (process.env.NODE_ENV === 'development' && this.amountEdited === 0) {
          this.initialize()
          this.validate()
        } else {
          // For each entry from props, create a new table row
          if (this.value !== null) {
            // For each timesheet table entry, create a new set 'obj'
            this.value.forEach(row => {
              let obj = {};

              // Only add attributes that fit an existing column header
              Object.entries(row).forEach(([key, value]) => {
                if (key in this.colValidation) {
                  this.$set(obj, key, value);
                } else if (['parsed', 'disabled', 'parsedValue'].includes(key)) {
                  this.$set(obj, key, value);
                } else if (key !== 'errors') {
                  console.log(
                    "Unrecognized in form field from cache: " +
                      `${key} - ${value}`
                  );
                }
              });

              // If the row was not empty, add it to the table
              if (Object.keys(obj).length > 0) {
                if (!('parsed' in obj)) {
                  this.$set(obj, 'parsed', true);
                }
                if (!('disabled' in obj)) {
                  this.$set(obj, 'disabled', true);
                }
                this.$set(obj, "errors", {});
                this.allEntries.push(obj);
              }
            });
          }
        }
      },
    },
  };
</script>
