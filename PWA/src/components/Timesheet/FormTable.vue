<template>
  <v-data-table class="elevation-1" :headers="headers" :items="allEntries">
    <!-- The header of the table -->
    <template v-slot:top>
      <v-toolbar flat>
        <v-toolbar-title>
          Service Delivered On:
        </v-toolbar-title>

        <v-spacer></v-spacer>

        <!-- Warning dialog upon editing a parsed field -->
        <v-dialog max-width="500px" v-model="displayWarning">
          <v-card>
            <v-card-title class="headline"
              >Do you want to edit this field?</v-card-title
            >

            <v-card-text>
              This field was parsed by AWS Textrack, based on the IDD timesheet
              that was uploaded. If you edit this field, the employer and
              provider <em>must</em> re-sign this form at the bottom before
              submission.
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>

              <v-btn color="red darken-1" text @click="closeWarning()">
                Cancel edit
              </v-btn>

              <v-btn color="green darken-1" text @click="warnContinue()">
                Edit field
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
                <v-icon v-else>mdi-lock-open</v-icon>
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
              <span class="headline">{{ formTitle }}</span>
            </v-card-title>

            <!-- The form area -->
            <v-card-text v-if="displayEditDialog">
              <v-container>
                <v-row
                  class="py-0 my-0"
                  v-for="field in [
                    'date',
                    'startTime',
                    'endTime',
                    'totalHours',
                  ]"
                  :key="field"
                >
                  <FormField
                    v-bind="colValidation[field]"
                    v-model="editedItem[field]"
                  />
                </v-row>

                <v-checkbox
                  label="Group? (y/n)"
                  true-value="Yes"
                  false-value="No"
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
              <v-btn color="red darken-1" text @click="close">
                Cancel
              </v-btn>
              <v-btn color="green darken-1" text @click="save">
                Save
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

    <template v-slot:item.startTime="{ item }">
      <v-container flat :class="getColor(item.errors, 'startTime')">
        {{ item.startTime }}
      </v-container>
    </template>

    <template v-slot:item.endTime="{ item }">
      <v-container flat :class="getColor(item.errors, 'endTime')">
        {{ item.endTime }}
      </v-container>
    </template>

    <template v-slot:item.totalHours="{ item }">
      <v-container flat :class="getColor(item.errors, 'totalHours')">
        {{ item.totalHours }}
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
          off-icon="lock_open"
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
  import FormField from "@/components/Timesheet/FormField";
  import rules from "@/components/Timesheet/FormRules.js";
  import time_functions from "@/components/Timesheet/TimeFunctions.js";

  export default {
    name: "FormTable",
    components: {
      FormField,
    },
    props: {
      // A .json file that is a section from the parsed uploaded IDD timesheet data
      value: {
        type: Array,
        default: null,
      },
      disabled: {
        type: Boolean,
        default: false,
      },
      modified: {
        type: Boolean,
        default: true,
      },
      parsed: {
        type: Boolean,
        default: false,
      },
      parsed_value: {
        type: Array,
        default: null,
      },
      // Reset to default props or no
      reset: {
        type: Boolean,
        default: false,
      },
    },
    data: function () {
      return {
        // Specify rules and hints for adding a new row to the table
        colValidation: {
          date: {
            counter: 10,
            disabled: false,
            hint: "YYYY-MM-DD",
            label: "Date",
            rules: rules["required"].concat(rules["date"], rules.maxLength(10)),
          },
          startTime: {
            counter: 8,
            disabled: false,
            hint: "HH:mm AM/PM",
            label: "Start/Time IN",
            rules: rules["required"].concat(
              rules["timeOfDay"],
              rules.maxLength(8)
            ),
          },
          endTime: {
            counter: 8,
            disabled: false,
            hint: "HH:mm AM/PM",
            label: "End/Time OUT",
            rules: rules["required"].concat(
              rules["timeOfDay"],
              rules.maxLength(8)
            ),
          },
          totalHours: {
            counter: 5,
            disabled: false,
            hint: "HH:mm",
            label: "Total Hours",
            rules: rules["required"].concat(rules["time"], rules.maxLength(5)),
          },
          group: {
            disabled: false,
          },
        },

        // Column headers and associated values for the table
        headers: [
          { text: "Date", align: "start", value: "date", sortable: false },
          { text: "Start/Time IN", value: "startTime", sortable: false },
          { text: "End/Time OUT", value: "endTime", sortable: false },
          { text: "Total Hours", value: "totalHours", sortable: false },
          { text: "Group?", value: "group", sortable: false },
          { text: "Actions", value: "actions", sortable: false },
        ],

        // Record the amount of edited parsed fields and added rows
        amtEdited: false,

        // Hide the warning popup for unlocking a parsed row or adding a row
        editingTable: false,
        displayWarning: false,

        // Hide the dialog popup for adding/editing a row
        displayEditDialog: false,
        formTitle: "Add/Edit Row",

        // All entries of the data table
        allEntries: [],

        // Index of the row that is currently being added/edited
        editedIndex: -1,

        // Default values for a new row in the table
        defaultItem: {
          date: "",
          timeIn: "",
          timeOut: "",
          timeTotal: "",
          group: "No",
          disabled: false,
          parsed: false,
          errors: {},
        },

        // Helper object for holding changes to a row in the table before
        // saving changes to that row
        editedItem: {
          date: "",
          startTime: "",
          endTime: "",
          totalHours: "",
          group: "No",
          disabled: false,
          parsed: false,
          errors: {},
        },

        // The last focused element before a dialog/popup appears
        // This allows for resuming tabbing after the dialog/popup closes
        focusedElement: null,
      };
    },

    watch: {
      // Reset table if parent component sends a reset signal
      reset() {
        this.initialize();
        this.validate();
      },
    },

    created: function () {
      this.initialize();
      this.validate();
    },

    methods: {
      // Parse .json from props into rows for the data table
      initialize() {
        // Reset the entries in the table & notify parent of change
        this.allEntries = [];
        this.$emit("disable-change", this.amtEdited * -1);
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
        if (item.group === "Yes") {
          item.group = "No";
        } else {
          item.group = "Yes";
        }
      },

      // Delete a single row of the table
      deleteItem(item) {
        const index = this.allEntries.indexOf(item);
        if (confirm("Are you sure you want to delete this item?")) {
          this.allEntries.splice(index, 1);
          this.validate();
        }

        // No need to notify parent, because an item can only be deleted if
        // the 'add row' button was unlocked or if a parsed row was unlocked
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
          this.$emit("input", this.allEntries);
        }
        this.validate();
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
        if (this.amtEdited < 1) {
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
        if (item.disabled === true) {
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
        var ret = 0;
        // The columns to check for validation (ex. exclude action, group)
        var cols = ["date", "startTime", "endTime", "totalHours"];

        // First check that each field has a valid value
        this.allEntries.forEach((entry) => {
          cols.forEach((col) => {
            // Reset the list of validation errors for this field for this row
            entry["errors"][col] = [];

            // Run the validation functions associated w/ this field
            if ("rules" in this.colValidation[col]) {
              this.colValidation[col]["rules"].forEach((rule) => {
                // If the validation function fails, add an error to field
                if (rule(entry[col]) !== true) {
                  entry["errors"][col].push("Invalid input");
                }
              });
            }
          });
        });

        // Next, check that the end time is after the start time
        this.allEntries.forEach((entry) => {
          // If the start and end times are valid, begin parsing
          if (
            entry["errors"]["startTime"].length === 0 &&
            entry["errors"]["endTime"].length === 0
          ) {
            var timeDifference = [0, 0]; // HH:mm
            var err = [0, 0]; // start & end; totalHours

            // Parse the times into a comparable format
            var start = time_functions.parseTime(entry["startTime"]);
            var end = time_functions.parseTime(entry["endTime"]);
            var total = time_functions.parseTime(entry["totalHours"]);

            // Check that the total hours field is correctly calculated
            /// Calculate total hours
            timeDifference = time_functions.subtractTime(start, end);

            if (timeDifference[0] < 0 || timeDifference[1] < 0) err[0] = 1;

            //// Compare the computed total hours with the user inputted hours
            if (
              timeDifference[0] !== total[0] ||
              timeDifference[1] !== total[1]
            ) {
              err[1] = 1;
            }

            // Create error messages
            if (err[0]) {
              entry["errors"]["startTime"].push("Invalid time interval");
              entry["errors"]["endTime"].push("Invalid time interval");
            }
            if (err[1]) {
              entry["errors"]["totalHours"].push("Incorrect calculation");
            }
          }
        });

        // Sort by date & time to check for overlapping time entries
        this.allEntries.sort(this.sortDateTime);

        return ret;
      },

      // Sort an entry by date and time
      // Adds errors to rows with overlapping date time entries
      sortDateTime(a, b) {
        // The columns to check for validation (ex. exclude action, group)
        var cols = ["date", "startTime", "endTime", "totalHours"];

        // Sort by date
        var dateComparison = time_functions.dateCompare(a.date, b.date);
        if (dateComparison !== 0) return dateComparison;

        // Dates are the same, sort by time such that a < b
        var aStart = time_functions.parseTime(a.startTime);
        var aEnd = time_functions.parseTime(a.endTime);
        var bStart = time_functions.parseTime(b.startTime);
        var timeDiff = time_functions.subtractTime(aStart, bStart);
        var order = 1;

        // If a starts after b starts, it appears before b
        if (timeDiff[0] >= 0 || timeDiff[1] >= 0) {
          order = -1;
        }

        // If a and b start or end at the same time, error
        if (a.startTime === b.startTime || a.endTime === b.endTime) {
          cols.forEach((col) => {
            a["errors"][col].push("Start or end time already exists!");
            b["errors"][col].push("Start or end time already exists!");
          });
        }

        // Check that times do not overlap
        if (order === -1) {
          //// a starts, then b starts
          timeDiff = time_functions.subtractTime(aEnd, bStart);
        } else {
          //// b starts, then a starts
          timeDiff = time_functions.subtractTime(aEnd, bStart);
        }

        if (timeDiff[0] < 0 || timeDiff[1] < 0) {
          cols.forEach((col) => {
            a["errors"][col].push("Time period conflicts with another row!");
            b["errors"][col].push("Time period conflicts with another row!");
          });
        }
        return order;
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
    },
  };
</script>
