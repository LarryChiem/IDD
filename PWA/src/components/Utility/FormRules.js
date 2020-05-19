// Generic form validation rules
// NOTE: When adding rules to a required field, put the 'required' rule
// at the end, or else form validation will display the wrong error
// message for an empty field
const rules = {
  required() {
    return (v) => !!v || "This field is required";
  },
  maxLength(max) {
    return (v) =>
      (v && v.toString().length <= max) ||
      "This field can't be over " + max + " characters";
  },
  alpha() {
    return (v) => /^[a-zA-Z]+$/.test(v) || "This field must be letters only";
  },
  alphanumeric() {
    return (v) =>
      /^[a-zA-Z0-9]+$/.test(v) || "This field must be letters or numbers";
  },
  numeric() {
    return (v) => /^[0-9]+(.[0-9]+)?$/.test(v) || "This field must be a number";
  },
  name() {
    return (v) =>
      /^([a-zA-Z.',]+( [a-zA-Z.',]+)+)$/.test(v) || "Firstname Lastname";
  },
  timeOfDay() {
    return (v) =>
      /^[0-1][0-9]:[0-6][0-9] [AaPp][Mm]$/.test(v) ||
      "This field must be in format HH:mm AM/PM";
  },
  time() {
    return (v) =>
      /^[0-9][0-9]:[0-6][0-9]$/.test(v) || "This field must be in format HH:mm";
  },
  monthYear() {
    return (v) =>
      /^[0-9]{4}-[01][0-9]$/.test(v) || "This field must be in format YYYY-MM";
  },
  date() {
    return (v) =>
      /^[0-9]{4}-[01][0-9]-[0123][0-9]$/.test(v) ||
      "This field must be in format YYYY-MM-DD";
  },
  email() {
    return (v) => /.+@.+\..+/.test(v) || "E-mail must be valid";
  },
};

export default rules;
