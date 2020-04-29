let time_functions = {
  // Parses a time field like HH:mm AM/PM into an array of time sections
  parseTime(time) {
    var ret = [];

    // Check that time is long enough
    if (!time) return null;
    if (time.length < 5) return null;

    // Separate the HH:mm part
    ret.push(parseInt(time.substr(0, 2)));
    ret.push(parseInt(time.substr(3, 2)));

    // Separate the AM/PM part if it exists
    if (time.length > 5) {
      ret.push(time.substr(6, 2));
    }
    return ret;
  },

  // Compare two date fields like YYYY-mm-dd
  dateCompare(start, end) {
    if (!start || !end) return 1;

    // start is after end
    if (start > end) return 1;

    // start is before end
    if (start < end) return -1;

    // star and end are the same days
    return 0;
  },

  // Find the difference between two time fields like [HH, mm, AM/PM]
  subtractTime(a, b) {
    var start = a.slice();
    var end = b.slice();
    var ret = [0, 0];

    // Check that a and b are valid
    if (!start || !end) return 0;

    // If add 12 hours to fields with PM
    if (start.length === 3 && start[2] === "PM") start[0] += 12;
    if (end.length === 3 && end[2] === "PM") end[0] += 12;

    // Calculate the time difference
    var ret_hr = 0;
    var ret_min = 0;
    ret_hr = end[0] - start[0];

    if (ret_hr > 0) {
      // start is earlier than end; start->end minutes
      ret_min = 60 - start[1] + end[1];
      ret_hr -= 1;
      if (ret_min >= 60) {
        ret_hr += 1;
        ret_min -= 60;
      }
    } else if (ret_hr < 0) {
      // start is later than end; end->start minutes
      ret_min = 60 - end[1] + start[1];
      ret_hr -= 1;
      if (ret_min >= 60) {
        ret_hr += 1;
        ret_min -= 60;
      }
      ret_min *= -1;
    } else {
      // start and end are in the same hour
      ret_min = end[1] - start[1];
    }
    ret[0] = ret_hr;
    ret[1] = ret_min;
    return ret;
  },
};

export default time_functions;
