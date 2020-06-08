<template>
  <v-footer color="indigo darken-2" padless>
    <v-container class="pa-0 mx-0" fluid>
      <v-row class="pa-0 ma-0">
        <v-col class="text-center pa-0 ma-0">
          <!-- Use a for loop to create buttons in the footer -->
          <v-btn
            class
            color="white"
            rounded
            text
            v-for="(link, title) in links"
            :key="title"
            :href="link"
            >{{ text_title(title) }}</v-btn
          >
        </v-col>
      </v-row>
      <v-row class="pa-0 ma-0">
        <v-col class="indigo py-1 text-center white--text">
          ©{{ new Date().getFullYear() }} |
          {{ $t("components_AppShell_AppFooter_phone") }}
          : 503.823.4000
        </v-col>
      </v-row>
    </v-container>
  </v-footer>
</template>

<script>
  import i18n from "@/plugins/i18n";

  export default {
    name: "AppFooter",
    data: function () {
      return {
        // A list of links for more info about the company
        links: {
          0: process.env.VUE_APP_ABOUT_US,
          1: process.env.VUE_APP_OPPORTUNITIES,
          2: process.env.VUE_APP_ACCESS,
        },
      };
    },
    methods: {
      text_title(id) {
        if (id == 0) return i18n.t("components_AppShell_AppFooter_about");
        else if (id == 1)
          return i18n.t("components_AppShell_AppFooter_opportunities");
        else if (id == 2) return i18n.t("components_AppShell_AppFooter_access");
        return i18n.t("translate_error");
      },
    },
  };
</script>
