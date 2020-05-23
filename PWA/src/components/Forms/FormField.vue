<template>
  <v-row class="mx-2" xl="4" lg="4" md="6" sm="12" xs="12">
    <v-col cols="1" v-if="this.parsed_value !== null">
      <!-- Lock/Unlock icon to enable editing or reset & lock a parsed field -->
      <v-checkbox
        hide-details
        off-icon="lock_open"
        on-icon="lock"
        v-model.lazy="isDisabled"
        @click.stop="askEdit($event)"
        @keyup.native.enter.stop="askEdit($event)"
      ></v-checkbox>

      <!-- Warning dialog upon editing a parsed field -->
      <v-dialog v-model="editDialog" max-width="500px">
        <v-card>
          <v-card-title class="headline"
            >Do you want to edit this field?</v-card-title
          >

          <v-card-text>
            This text was created based on the IDD timesheet that was uploaded.
            Sometimes, the app can't quite read your handwritting correctly, and
            you will need to edit before sumbitting. This will ensure that your
            timesheet is not returned as incorrect. Please make any corrections
            to match your timesheet exactly by selecting "Edit Field".
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>

            <v-btn color="red white--text" @click="closeDialog()">
              Cancel edit
            </v-btn>

            <v-btn
              color="green white--text"
              @click.stop="
                changeDisable();
                closeDialog();
              "
            >
              Edit field
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <!-- END warning dialog upon editing a parsed field -->
    </v-col>

    <!-- Form Field -->
    <!-- Check if field is a checkbox -->
    <v-col v-if="this.field_type === 1">
      <v-checkbox
        :label="label"
        :disabled="isDisabled"
        :rules="rules"
        :input-value="value"
        @change="$emit('input', !value)"
        @keyup.native.enter="$emit('input', !value)"
        lazy
      ></v-checkbox>
    </v-col>

    <!-- Is not a checkbox, check if this field is a textarea -->
    <v-col v-else-if="this.rows > 1 || this.auto_grow">
      <v-textarea
        :auto-grow="auto_grow"
        :counter="counter"
        :filled="isDisabled"
        :outlined="!disabled"
        :hint="hint"
        :label="label"
        :readonly="disabled"
        :rows="rows"
        :rules="rules"
        :value="value"
        @input="$emit('input', $event)"
        lazy
      ></v-textarea>
    </v-col>

    <!-- Is not a textarea, so it is a text-field -->
    <v-col v-else>
      <v-text-field
        :counter="counter"
        :filled="isDisabled"
        :outlined="!isDisabled"
        :hint="hint"
        :label="label"
        :readonly="isDisabled"
        :rules="rules"
        :value="value"
        @input="$emit('input', $event)"
        lazy
      ></v-text-field>
    </v-col>
    <!-- End form Field -->
  </v-row>
</template>

<script>
  export default {
    name: "FormField",
    props: {
      // does the text field grow in size as more text is entered
      auto_grow: {
        type: Boolean,
        Default: false
      },
      // max strlen for text field
      counter: {
        default: 25
      },
      // 0 - text-field/textarea
      // 1 - checkbox
      field_type: {
        type: Number,
        default: 0
      },
      // appears below text field
      hint: {
        type: String,
        default: ""
      },
      // appears above text field
      label: {
        type: String,
        default: ""
      },
      // value parsed from .json
      parsed_value: {
        default: null
      },
      // Reset to default props or no
      reset: {
        type: Boolean,
        default: false
      },
      // size of this text field
      rows: {
        type: Number,
        Default: 1
      },
      // validation rules for text field
      rules: {
        type: Array,
        Default: null
      },
      // value entered in text field
      value: {
        type: [String, Boolean, Number],
        Default: null
      },
      willResign: {
        type: Boolean,
        Default: false
      },
      disabled: {
        type: Boolean,
        Default: false
      }
    },

    // Manage fields that change on this page
    data: function() {
      return {
        isDisabled: this.disabled,
        editDialog: false,
        focusedElement: null
      };
    },

    watch: {
      reset() {
        if (this.parsed_value !== null) {
          this.isDisabled = true;
        } else {
          this.$emit("input", null);
        }
      },
      disabled(val) {
        this.isDisabled = val;
      }
    },

    // Do an action or communicate info to parent component upon a certain
    // event
    methods: {
      // For a parsed field only:
      // Display warning message before unlocking field for editing
      // Do not display warning if re-locking field
      askEdit(event) {
        this.focusedElement = event.target;
        if (this.isDisabled === true && this.willResign === false) {
          this.editDialog = true;
        } else {
          this.changeDisable();
        }
      },

      // 300 because click on button enter, so no infinite
      closeDialog() {
        this.editDialog = false;
        setTimeout(() => {
          this.focusedElement.focus();
        }, 300);
      },

      // For a parsed field only:
      // Unlock a disabled parsed field
      // Else, reset field to its parsed value & disable the field
      changeDisable() {
        this.isDisabled = !this.isDisabled;
        if (this.isDisabled === true) {
          this.$emit("input", this.parsed_value);
          this.$emit("disable-change", -1);
        } else {
          this.$emit("disable-change", 1);
        }
      }
    }
  };
</script>
