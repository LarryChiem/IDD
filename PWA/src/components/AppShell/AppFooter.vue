<template>
  <v-footer color="indigo darken-2" padless>
    <v-row justify="center" no-gutters>
      <!-- Use a for loop to create buttons in the footer -->
      <v-btn
        class=""
        color="white"
        rounded
        text
        v-for="(link, title) in links"
        :key="title"
        :href="link"
      >
        {{ title }}
      </v-btn>

      <!-- On a new line (12 cols per line), add copyright and contact info -->
      <v-col class="indigo py-1 text-center white--text" cols="12">
        ©{{ new Date().getFullYear() }} | Multnomah County General Information
        Line: 503.823.4000
      </v-col>
    </v-row>
  </v-footer>
</template>

<script>
  export default {
    name: "AppFooter",
    data: function () {
      return {
        // A list of links for more info about the company
        links: {
          "About Us":
            "https://multco.us/multnomah-county/about-multnomah-county",
          Opportunities: "https://multco.us/jobs",
          Access:
            "https://multco.us/diversity-equity/ada-policy-notice-and-disability-complaint-process",
        },
      };
    },
  };
</script>
