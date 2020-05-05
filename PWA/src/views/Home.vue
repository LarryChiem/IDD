<template>
  <!-- Background image with a top->bottom gradient -->
  <v-img
    class="fill-height"
    gradient="to top, rgba(150,150,150,.3), rgba(255,255,255,.98)"
    max-height="100%"
    src="https://media.giphy.com/media/gg4BGWfydPAA0/giphy.gif"
  >
    <v-container>
      <v-row align="center" justify="center">
        <v-col cols="12" md="4" sm="8">
          <v-img
            contain
            height="200"
            :src="require('../assets/icons/logo_full.svg')"
          />
        </v-col>
      </v-row>

      <v-spacer></v-spacer>
      <v-spacer></v-spacer>

      <v-row align="center" justify="center">
        <v-col cols="12" md="4" sm="8">
          <v-card class="mx-auto" max-width="250" to="/timesheet" link>
            <v-row>
              <v-col>
                <v-card-text>
                  <h4>
                    Upload Timesheet
                  </h4>
                </v-card-text>
              </v-col>
            </v-row>
            <v-row>
              <v-col>
                <v-icon x-large color="success">add_circle</v-icon>
              </v-col>
            </v-row>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-img>
</template>

<style lang="scss" scoped>
  .v-card {
    text-decoration: none;
  }
  .v-icon {
    display: flex;
    justify-content: center;
    align-items: center;
  }
  h4 {
    display: flex;
    text-align: center;
    justify-content: center;
    align-items: center;
  }
</style>

<script>
  export default {
    name: "Home",
    props: {
      source: String,
    },
  };
</script>
