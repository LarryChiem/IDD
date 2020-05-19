// To be used in views/Timesheet.vue
export const FORM = {
  OR004_MILEAGE: 1,
  OR507_RELIEF: 2,
  OR526_ATTENDANT: 3,
};

// Types of forms
export const FORM_TYPE = {
  OR004_MILEAGE: "Mileage",
  OR507_RELIEF: "ServiceDelivered",
  OR526_ATTENDANT: "ServiceDelivered",
};

// To be used in views/Timesheet.vue
export const FILE = {
  INIT: 1,
  SUCCESS: 2,
  FAILURE: 3,
};

// Supported datetime strings
export const TIME = {
  FULL_DATE: "YYYY-MM-DD HH:mm A",
  YEAR_MONTH_DAY: "YYYY-MM-DD",
  YEAR_MONTH: "YYYY-MM",
  TIME_12: "HH:mm A",
  TIME_24: "HH:mm",
  ERROR: -0,
};
