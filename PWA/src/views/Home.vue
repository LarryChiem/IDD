<template>
  <!-- Background image with a top->bottom gradient -->
  <!-- From https://commons.wikimedia.org/wiki/File:Portland_and_Mt._Hood_from_Pittock_Mansion.jpg -->
  <!-- This file is licensed under the Creative Commons Attribution-Share Alike 4.0 International license. -->
  <v-img
    class="fill-height"
    gradient="to top, rgba(255,255,255,.10), rgba(0,0,0,0)"
    max-height="100%"
    src="@/assets/blurry_card_portland.jpg"
  >
    <v-container>
      <v-row align="center" justify="center">
        <v-col cols="12" md="4" sm="8">
          <v-img
            contain
            height="200"
            :src="require('@/assets/icons/logo_full.svg')"
          />
        </v-col>
      </v-row>

      <v-spacer></v-spacer>
      <v-spacer></v-spacer>

      <v-container fluid>
        <v-row align="center" justify="center">
          <v-col
            cols="12"
            md="4"
            sm="8"
            v-for="card in cards"
            :key="card.title"
          >
            <v-card
              class="mx-auto"
              elevation="4"
              max-width="250"
              :to="card.link"
              link
            >
              <template>
                <v-img :src="card.src" height="200px"> </v-img>
                <v-card-text>
                  <h4>
                    {{ card.title }}
                  </h4>
                  <v-icon x-large :color="card.iconColor">{{
                    card.icon
                  }}</v-icon>
                </v-card-text>
              </template>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
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
  const pic_timesheet = require("@/assets/card_timesheet.jpg");
  const pic_burnside = require("@/assets/card_burnside.jpg");

  export default {
    name: "Home",
    props: {
      source: String,
    },
    data: () => ({
      cards: [
        {
          title: "Upload Timesheet",
          src: pic_timesheet,
          link: "/timesheet",
          icon: "add_circle",
          iconColor: "success",
        },
        {
          title: "About",
          src: pic_burnside,
          link: "/about",
          icon: "info",
          iconColor: "warning",
        },
      ],
    }),
  };
</script>
