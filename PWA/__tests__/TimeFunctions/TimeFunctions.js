import {
  subtractTime,
  milliToFormat,
} from "@/components/Utility/TimeFunctions.js";
import { TIME } from "@/components/Utility/Enums.js";
var moment = require("moment");

let date_valid = "2000-01-01";

let subtract_null = [
  [null, null],
  [date_valid, null],
  [null, date_valid],
];

let subtract_malformat = [
  ["2020-12-2", "2020-12-20", TIME.YEAR_MONTH_DAY, TIME.ERROR],
  ["2020-12-20", "2020-12-2", TIME.YEAR_MONTH_DAY, TIME.ERROR],
  ["20s0-10-20", "2020-12-20", TIME.YEAR_MONTH_DAY, TIME.ERROR],
  ["2020-10-20", "20s0-12-20", TIME.YEAR_MONTH_DAY, TIME.ERROR],
  ["20:1", "20:20", "HH:MM", TIME.ERROR],
  ["20:s0", "20:20", "HH:MM", TIME.ERROR],
  ["20.20", "20:20", "HH:MM", TIME.ERROR],
  ["20:20", "20:1", "HH:MM", TIME.ERROR],
  ["20:20", "20:s0", "HH:MM", TIME.ERROR],
  ["20:20", "20:20", "HH:MM", TIME.ERROR],
  ["10:20 A", "10:20 AM", "HH:MM A", TIME.ERROR],
  ["10:s0 AM", "10:20 AM", "HH:MM A", TIME.ERROR],
  ["10:20", "10:20 AM", "HH:MM A", TIME.ERROR],
  ["13:20 AM", "10:20 AM", "HH:MM A", TIME.ERROR],
  ["00:20 AM", "10:20 AM", "HH:MM A", TIME.ERROR],
  ["10:20 am", "10:20 AM", "HH:MM A", TIME.ERROR],
  ["10:20 AM", "10:20 A", "HH:MM A", TIME.ERROR],
  ["10:20 AM", "10:s0 AM", "HH:MM A", TIME.ERROR],
  ["10:20 AM", "10:20", "HH:MM A", TIME.ERROR],
  ["10:20 AM", "13:20 AM", "HH:MM A", TIME.ERROR],
  ["00:20 AM", "00:20 AM", "HH:MM A", TIME.ERROR],
  ["10:20 AM", "10:20 am", "HH:MM A", TIME.ERROR],
];

describe("'subtractTime' function", () => {
  test.each(subtract_null)(
    "Given %p and %p as the first two arguments, return an error value",
    (firstArg, secondArg) => {
      expect(subtractTime(firstArg, secondArg, "")).toBe(TIME.ERROR);
    }
  );

  test.each(subtract_malformat)(
    "Given malformatted arguments %p and %p with expected format %p, return an error value",
    (firstArg, secondArg, thirdArg) => {
      expect(subtractTime(firstArg, secondArg, thirdArg)).toBe(TIME.ERROR);
    }
  );

  test.each`
    a                        | b                        | c                      | expectedFormat         | expectedValue
    ${"2020-12-20"}          | ${"2020-12-20"}          | ${TIME.YEAR_MONTH_DAY} | ${TIME.YEAR_MONTH_DAY} | ${"0000-00-00"}
    ${"2020-12-20"}          | ${"2020-12-20"}          | ${TIME.YEAR_MONTH_DAY} | ${TIME.YEAR_MONTH_DAY} | ${"0000-00-00"}
    ${"2020-12-20"}          | ${"2020-12-21"}          | ${TIME.YEAR_MONTH_DAY} | ${TIME.YEAR_MONTH_DAY} | ${"0000-00-01"}
    ${"2020-12-20 10:00 AM"} | ${"2020-12-20 10:00 AM"} | ${TIME.FULL_DATE}      | ${"YYYY-MM-DD HH:mm"}  | ${"0000-00-00 00:00"}
    ${"2020-12-20 10:00 AM"} | ${"2020-12-20 10:30 AM"} | ${TIME.FULL_DATE}      | ${"YYYY-MM-DD HH:mm"}  | ${"0000-00-00 00:30"}
    ${"2020-12-20 10:00 AM"} | ${"2020-12-20 10:00 PM"} | ${TIME.FULL_DATE}      | ${"YYYY-MM-DD HH:mm"}  | ${"0000-00-00 12:00"}
    ${"2020-12-20 10:00 AM"} | ${"2020-12-20 10:30 PM"} | ${TIME.FULL_DATE}      | ${"YYYY-MM-DD HH:mm"}  | ${"0000-00-00 12:30"}
    ${"10:00 AM"}            | ${"10:00 AM"}            | ${TIME.TIME_12}        | ${TIME.TIME_24}        | ${"00:00"}
    ${"10:10 AM"}            | ${"10:45 AM"}            | ${TIME.TIME_12}        | ${TIME.TIME_24}        | ${"00:35"}
    ${"10:10 AM"}            | ${"10:45 PM"}            | ${TIME.TIME_12}        | ${TIME.TIME_24}        | ${"12:35"}
    ${"10:45 AM"}            | ${"10:10 AM"}            | ${TIME.TIME_12}        | ${TIME.TIME_24}        | ${"-00:35"}
    ${"10:45 PM"}            | ${"10:10 AM"}            | ${TIME.TIME_12}        | ${TIME.TIME_24}        | ${"-12:35"}
    ${"10:00"}               | ${"10:00"}               | ${TIME.TIME_24}        | ${TIME.TIME_24}        | ${"00:00"}
    ${"10:00"}               | ${"10:40"}               | ${TIME.TIME_24}        | ${TIME.TIME_24}        | ${"00:40"}
    ${"10:00"}               | ${"22:40"}               | ${TIME.TIME_24}        | ${TIME.TIME_24}        | ${"12:40"}
    ${"10:40"}               | ${"10:00"}               | ${TIME.TIME_24}        | ${TIME.TIME_24}        | ${"-00:40"}
    ${"22:40"}               | ${"10:00"}               | ${TIME.TIME_24}        | ${TIME.TIME_24}        | ${"-12:40"}
  `(
    "Given valid arguments $a and $b with expected format $c, return $expectedValue in format $expectedFormat",
    ({ a, b, c, expectedFormat, expectedValue }) => {
      var response = subtractTime(a, b, c);
      expect(milliToFormat(response, expectedFormat)).toBe(expectedValue);
    }
  );
});
