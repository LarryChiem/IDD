# IDD Timesheet Project

This project is to help facilitate third parties uploading timesheet images
and having the image digitized into a timesheet object using services
such as AWS Textract or Google Vision.

## PWA

This directory is the PWA portion of the project that is used by the third
party to upload their timesheets.

## Appserver

This directory contains the backend API that is used in conjunction with the
PWA to receive the image, digitize the information, and send responses back
to the PWA.

## Admin UI

This directory contains the administrative side which allows a company to 
examine the digitized information and approve or disapprove submissions.

## License

This software is licensed under the MIT License. To view the license read the 
LICENSE file found at the root of this project.
