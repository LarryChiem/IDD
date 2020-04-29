// Generic form validation rules
let rules = {
  required: [(v) => !!v || "This field is required"],
  maxLength(max) {
    return (v) =>
      (v && v.toString().length <= max) ||
      "This field must be less than " + (max + 1) + " characters";
  },
  alphanumeric: [
    (v) => /^[a-zA-Z0-9]+$/.test(v) || "This field must be letters or numbers",
  ],
  numeric: [(v) => /^[0-9]+$/.test(v) || "This field must be a number"],
  timeOfDay: [
    (v) =>
      /^[0-1][0-9]:[0-6][0-9] [AaPp][Mm]$/.test(v) ||
      "This field must be in format HH:mm AM/PM",
  ],
  time: [
    (v) =>
      /^[0-9][0-9]:[0-6][0-9]$/.test(v) || "This field must be in format HH:mm",
  ],
  monthYear: [
    (v) =>
      /^[0-9]{4}-[01][0-9]$/.test(v) || "This field must be in format YYYY-MM",
  ],
  date: [
    (v) =>
      /^[0-9]{4}-[01][0-9]-[0123][0-9]$/.test(v) ||
      "This field must be in format YYYY-MM-DD",
  ],
  email: [(v) => /.+@.+\..+/.test(v) || "E-mail must be valid"],
};

export default rules;
