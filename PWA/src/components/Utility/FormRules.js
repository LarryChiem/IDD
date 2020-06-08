// Generic form validation rules

import i18n from "@/plugins/i18n";

const rules = {
  required() {
    return (v) => !!v || i18n.t("rules_required");
  },
  maxLength(max) {
    return (v) =>
      (v && v.toString().length <= max) ||
      i18n.t("rules_max0") + max + i18n.t("rules_max1");
  },
  alpha() {
    return (v) => /^[a-zA-Z]+$/.test(v) || i18n.t("rules_alpha");
  },
  alphanumeric() {
    return (v) => /^[a-zA-Z0-9]+$/.test(v) || i18n.t("rules_alphanumeric");
  },
  numeric() {
    return (v) => /^[0-9]+(.[0-9]+)?$/.test(v) || i18n.t("rules_numeric");
  },
  name() {
    return (v) =>
      /^([a-zA-Z.',]+( [a-zA-Z.',]+)+)$/.test(v) || i18n.t("rules_name");
  },
  timeOfDay() {
    return (v) =>
      /^[0-1][0-9]:[0-5][0-9] [AaPp][Mm]$/.test(v) || i18n.t("rules_timeOfDay");
  },
  time() {
    return (v) => /^[0-9][0-9]:[0-5][0-9]$/.test(v) || i18n.t("rules_time");
  },
  monthYear() {
    return (v) => /^[0-9]{4}-[01][0-9]$/.test(v) || i18n.t("rules_monthYear");
  },
  date() {
    return (v) =>
      /^[0-9]{4}-[01][0-9]-[0123][0-9]$/.test(v) || i18n.t("rules_date");
  },
  email() {
    return (v) => /.+@.+\..+/.test(v) || i18n.t("rules_date");
  },
};

export default rules;
