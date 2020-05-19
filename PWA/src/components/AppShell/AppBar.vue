<template>
  <v-app-bar app color="indigo" dark>
    <!-- Button to toggle the navigation drawer -->
    <v-app-bar-nav-icon @click.stop="onInput" />

    <!-- Application name -->
    <v-toolbar-title>
      <v-btn x-large title text link to="/">
        IDD Timesheet Submission
      </v-btn>
    </v-toolbar-title>

    <v-spacer></v-spacer>

    <!-- Link to user help manuals -->
    <v-btn
      color="indigo lighten-1"
      depressed
      href="https://github.com/Capstone-Team-C/IDD"
      target="_blank"
    >
      Help
      <v-icon>mdi-open-in-new</v-icon>
    </v-btn>
  </v-app-bar>
</template>

<script>
  export default {
    name: "AppBar",
    props: {
      open: {
        type: Boolean,
        default: true,
      },
    },
    data: function () {
      return {
        // Display toggle; if true, display navigation drawer, else hide
        drawerOpen: this.open,
      };
    },
    // Parent -> Child communication
    watch: {
      // Change should come from the NavigationDrawer closing
      open(newVal) {
        this.drawerOpen = newVal;
      },
    },
    // Child -> Parent communication
    methods: {
      // Change 'isOpen' upon pressing the app-bar-nav-icon
      onInput() {
        this.$emit("drawer-change", !this.drawerOpen);
      },
    },
  };
</script>
