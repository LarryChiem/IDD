import { TIME } from "@/components/Utility/Enums.js";

var moment = require("moment");
var momentDurationFormatSetup = require("moment-duration-format");
momentDurationFormatSetup(moment);

/**
 * function:: subtractTime(start, end, format)
 *      Find the difference of two datetimes in milliseconds.
 * Parameters:
 *      * `start`: <String> Subtrahend; the term to the right of the minus
 *      * `end`: <String> Minuend; the term to the left of the minus
 *      * `format`: <String> The enum representing the expected format of
 *          the `start` and `end`
 * Returns:
 *      * <Number> The amount of milliseconds passed since `start` to `end`
 */
export const subtractTime = (start, end, format) => {
  if (start == undefined || end == undefined || format == undefined)
    return TIME.ERROR;

  const formattedStart = moment(start, format, true);
  const formattedEnd = moment(end, format, true);

  if (!formattedStart.isValid() || !formattedEnd.isValid()) {
    return TIME.ERROR;
  }

  const difference = moment.duration(formattedEnd.diff(formattedStart));
  if (difference === 0) {
    return 0;
  }

  return difference.asMilliseconds();
};

/**
 * function:: milliToFormat(milli, format)
 *      Convert milliseconds to a specified datetime format
 * Parameters:
 *      * `milli`: <Number> An duration of time expressed in milliseconds
 *      * `format`: <String> The enum representing the desired output fomat
 * Returns:
 *      * <String> `milli` expressed as a during in the format `format`
 */
export const milliToFormat = (milli, format) => {
  if (milli == undefined) return TIME.ERROR;
  let ret = "";
  if (milli < 0) ret += "-";

  const dur = moment.duration(Math.abs(milli));
  ret += dur.format(format, { trim: false, useGrouping: false });

  return ret;
};
