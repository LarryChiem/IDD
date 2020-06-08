# Vue.js Progressive Web App

Our Progressive Web App (PWA) is built with [Vue.js](https://vuejs.org/) off of [Node.js](https://nodejs.org/en/), using the [Express](https://expressjs.com/) framework to simplify the process for starting the production server. 

The packages our PWA uses are managed with [NPM.js](https://www.npmjs.com/).

Our PWA uses the [Vue CLI](https://cli.vuejs.org/guide/) built on top of wepback for rapid development.

## Project setup
### Cloning the project repository
Before continuing, be sure to clone this project onto your device, such that the project exists locally on your machine.

1) In an internet browser, navigate to this project's git repository.
2) Copy the git command for cloning this project repository.
3) In a `git-bash`/`cmd window`/`PowerShell window`/`etc.`, navigate to the directory that will contain the project files.
4) Run the command from step 2.
5) Navigate to the PWA root directory, which is the `PWA/` folder.

***
### Installing Node.js and NPM
#### Windows
1) Download node via the [Node.js Installer](https://nodejs.org/en/download/)
2) Once the installer finishes, open the downloaded file and run the Node.js Setup Wizard.
3) The installer will prompt your for the installation location and to select components to include or remove from the installation.
4) In **Custom Setup** select **npm package manager** as the recommended package manager for Node.js.
5) Finally, click the Install button to run the installer. When it finishes, click Finish.

##### Verify the Installation
Open a git-bash shell (preferably), a command prompt, or PowerShell and enter the following:
```
node -v
npm -v
```
The system should display the Node.js and NPM version installed.
***
#### Linux
1) Open your terminal.
2) To install Node.js, use the following command
```
sudo apt install nodejs
```
3) To install NPM, use following command
```
sudo apt install npm
```
4) Once installed, verify that NPM and Node.js are installed by checking the installed versions:
```
node -v
npm -v
```
The system should display the Node.js and NPM version installed.
***
#### Mac
1) Download [Node.js](https://nodejs.org/en/download/) for macOS.
2) When the file finishes downloading, locate it in **Finder** and double-click on it.
3) Complete the installation process. 
4) Verify instillation by opening a terminal and type
```
node -v
npm -v
```
#### Install Node.js and NPM with Homebrew
If you have Homebrew installed on your device, open a terminal and type:
```
brew update
brew install node
```
***

### Setting up PWA-specific environment variables
This app makes use of [Vue environment variables](https://cli.vuejs.org/guide/mode-and-env.html), an effective method of configuring Node.js applications. You can specify env variables by placing the following files the PWA root directory: 

```sh
.env                # loaded in all cases
.env.local          # loaded in all cases, ignored by git
.env.[mode]         # only loaded in specified mode
.env.[mode].local   # only loaded in specified mode, ignored by git
```
> :warning: **Do not store any secrets (such as private API keys) in your app**: Environment variables are embedded into the build, meaning anyone can view them by inspecting your app's files.



Please note that all env variables in this Vue app must start with `VUE_APP`, a requirement when running `vue-cli-service`.

You can access env variables in the application code:

```js
console.log(process.env.VUE_APP_NOT_SECRET_CODE)
```

We have included an `.env.example` file with env variables used in the code.  For local development, we recommend creating a `.env` file that includes at least the same variables in the `.env.example` file. You will need to specify these env variables before building the app.

When deploying the app for production, we recommend specifying these env variables in the app settings of the service you are hosting the app on.

## Running our Vue.js project locally
1) Move to the PWA root directory in the project directory.
2) Install necessary packages with npm:
```sh
npm install
```
3) Create an `.env` file with at least the same variables in the `.env.example` file and configure the variables to your needs.
```sh
cp .env.example .env
```
```
VUE_APP_SERVER_URL=https://your.website.net/
VUE_APP_BUG_REPORT=https://github.com/your_team/your_repo/issues
VUE_APP_ABOUT_US=https://you_organization.us/about-us
VUE_APP_OPPORTUNITIES=https://your_organization.us/jobs
VUE_APP_ACCESS=https://your_organization.us/access
```
4) Build the Vue project (_only if running the production server_):
```
npm run build
```
  - This command creates the `dist/` directory, which contains an optimized version of the files from the `public/` and `src/` directories

5) Serve the Vue project:

```
npm run start    #Start the production server
npm run serve    #Start the development server
```
6) Open the project in your browser with the designated **localhost** url 
```
production server: http://localhost:ENTER_PORT_HERE
development server: http://localhost:8080
``` 
_NB: Set the 'PORT' variable to '8080' in the .env file to enable connection with the Appserver on the production server._

***
### Other Scripts
NPM scripts specified in the `package.json` file are basically macros, which may be executed via npm by running `npm run script_name`. The following list details the commands and scripts that exist for this project.
- `npm run build; npm run serve;`
	- Start the production server
- `npm run serve`
	- Start the development server
- `npm test` 
	- Runs the unit tests in the `__tests__/` directory
- `npm list`
	- View the node modules and dependencies
- `prettier path/to/directory/to/format --write`
	- Format the files in the specified directory
- `npm run lint`
	- Fixes the formatting of files

## Validation
The unit tests for this project are located in the `__tests__` folder. The `npm test` script will run all unit tests for this project. This project uses the [Jest](https://vue-test-utils.vuejs.org/guides/testing-single-file-components-with-jest.html) testing framework to execute unit tests.

To run all unit tests, run the following command: `npm test`. To run a specific test or set of tests, run the following command **from the PWA root directory**: `jest __tests__/path/to/test/dir`.

The configurations for jest are in the `package.json` file -- search for 'jest'.

## Locales with i18n
This project uses the `i18n` module for handling the displaying and switching of various languages. Translations are currently specified in the `src/plugins/i18n.js` file. In every component, validation rule, and form field hints, any text is replaced with a variable recognizable to `i18n`, which displays the correct translation. 

After importing the `i18n` plugin in a vue component, these variables can be either directly used in the HTML and script section or, in the case of dynamically rendered HTML, returned in a switch statement:

```html
<!-- HTML section of a Vue component -->
<!-- Directly used in the Vue HTML -->
<p>{{ $t("insert_variable_name_here") }}</p>

<!-- Dynamically loaded via js -->
<v-btn v-for="(link, title) in links" :key="title":href="link">
    {{ text_title(title) }}
</v-btn>

```
```js
// Script section of the above Vue component
<script>
  import i18n from "@/plugins/i18n";
...
    data: function () {
      return {
        links: { 0: link1.com, 1: link2.com, 2: link3.com },
      };
    },
    methods: {
      text_title(id) {
        if (id == 0) return i18n.t("link1_text");
        else if (id == 1) return i18n.t("link2_text");
        else if (id == 2) return i18n.t("link3_text");
        return i18n.t("translate_error_text");
      },
    },
...
</script>
```

## Adding a new form
The files directly related to a form are stored in the `src/components/Forms` directory, but there are some other files which must be edited to include the form throughout the PWA. _NB: This assumes that the Appserver is already configured to parse the new form._

1) In the `src/components/Forms` director, create a new directory to contain the files of the new form. Place the component's .vue file in this directory, as well as a .json file specifying for each of the form's fields: hints, labels, validation rules, maximum char counter, and other props for the `FormField` component.

2) In the `src/components/Utility/Enums.js` file, append the new form's name to the `FORM` and `FORM_TYPE` enums.

3) In the `src/views/Timesheet.vue` file, near the botton of the HTML section, add a new `<v-col v-else-if=""> <NewFormHere /> </v-col>` to dynamically render the new form, depending on the user's form selection.

4) In the `src/components/Forms/ConfirmSubmission.vue` file, in the computed section, add the Appserver submission url to the `url` variable. If there is a table component in the form, update the `sheetType` variable in the `formatData` method to include the sheet name specified in the .json file from step 1.

5) In the `src/store/modules/Forms/` directory, create a .js file to specify the vuex store for the new form component.

6) In the `src/store/index.js` file, import the file created from the previous step, and append it to the 'modules' of the vuex store.

7) In the following files, update 'mapMutations', 'mapFields', and reset function to include the new form where necessary:
- `src/components/Forms/ConfirmSubmission.vue`
- `src/views/Timesheet.vue`

## Push Deployment onto Azure (not recommended)
- Build the project via `npm run build`.
- Ensure that the web.manifest file from the `public/`  directory was copied into the generated `dist/` folder, as this is where the website will be served.
- In VS Code, download the Azure extension.
- Press `Ctrl+Shift+P`.
- Type "deploy" and select the first option.
- Follow through until it deploys.
- Check the Deployments (in the navbar to the left) to ensure that the project built correctly.
- Check the output logs to see if any errors occured during the deployment proccess.

