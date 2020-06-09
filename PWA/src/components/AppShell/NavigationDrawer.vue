<template>
  <v-navigation-drawer
    app
    color="grey lighten-4"
    v-model="drawerOpen"
    @input="onInput"
  >
    <v-list dense>
      <!-- Link to homepage -->
      <v-list-item link to="/">
        <v-list-item-action>
          <v-icon>mdi-home</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{
            $t("components_AppShell_NavigationDrawer_home")
          }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <!-- Link to IDD timesheet submition page-->
      <v-list-item link to="/timesheet">
        <v-list-item-action>
          <v-icon>mdi-note-plus</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{
            $t("components_AppShell_NavigationDrawer_upload")
          }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <!-- Link to the about page -->
      <v-list-item link to="/about" @click.native="$scrollToTop">
        <v-list-item-action>
          <v-icon>mdi-information-outline</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{
            $t("components_AppShell_NavigationDrawer_about")
          }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <!-- Link to the about page -->
      <v-tooltip bottom>
        <template v-slot:activator="{ on }">
          <v-list-item :href="bugReport" target="_blank" v-on="on">
            <v-list-item-action>
              <v-icon>mdi-bug</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>{{
                $t("components_AppShell_NavigationDrawer_bug")
              }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
        <span>{{ $t("components_AppShell_NavigationDrawer_bug") }}</span>
      </v-tooltip>
    </v-list>
  </v-navigation-drawer>
</template>

<script>
  export default {
    name: "NavigationDrawer",
    props: {
      // Display toggle value passed in from the parent component
      open: {
        type: Boolean,
        default: false,
      },
    },
    data: function () {
      return {
        // Display toggle; if true, display navigation drawer, else hide
        drawerOpen: this.open,
        bugReport: process.env.VUE_APP_BUG_REPORT,
      };
    },
    // Parent -> Child communication
    watch: {
      // Change should come from pressing a button on the AppBar
      open(newVal) {
        this.drawerOpen = newVal;
      },
    },
    // Child -> Parent communication
    methods: {
      // Change 'isOpen' upon closing the navigation drawer
      onInput(isOpen) {
        this.$emit("drawer-change", isOpen);
      },
    },
  };
</script>
