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
                    {{ text_title(card.title) }}
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
      <div class="text-center">
        <v-bottom-sheet hide-overlay :value="modalOpen">
          <v-alert
            color="light-green darken-3"
            class="py-3 my-0 pr-6"
            border="top"
            tile
            dismissible
            v-model="modalOpen"
          >
            <template v-slot:prepend>
              <div
                class="pa-1 mr-6"
                style="
                  border-radius: 4px;
                  background-color: rgba(255, 255, 255, 0.7);
                "
              >
                <v-img
                  max-width="5vw"
                  max-height="5vw"
                  :src="pic_logo"
                  contain
                />
              </div>
            </template>
            <v-row>
              <v-col justify="center" class="py-0 my-0 white--text">
                This website looks better if you save it onto your device
                <v-btn
                  @click.stop="promptInstall"
                  class="grey darken-3 ml-5"
                  small
                  dark
                  depressed
                  rounded
                >
                  Save
                </v-btn>
              </v-col>
            </v-row>
          </v-alert>
        </v-bottom-sheet>
      </div>
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
  import i18n from '@/plugins/i18n';
  import { VuePwaInstallMixin } from "vue-pwa-install";

  const pic_timesheet = require("@/assets/card_timesheet.jpg");
  const pic_burnside = require("@/assets/card_burnside.jpg");
  const pic_logo = require("@/assets/icons/logo_short.svg");

  export default {
    name: "Home",
    mixins: [VuePwaInstallMixin],
    props: {
      source: String,
    },
    data: () => ({
      cards: [
        {
          title: 0,
          src: pic_timesheet,
          link: "/timesheet",
          icon: "add_circle",
          iconColor: "success",
        },
        {
          title: 1,
          src: pic_burnside,
          link: "/about",
          icon: "info",
          iconColor: "warning",
        },
      ],
      deferredPrompt: false,
      modalOpen: false,
      pic_logo: pic_logo,
    }),
    created() {
      // If the client's browser attempts to prompt the client to download the PWA (aka. chrome),
      // block this attempt and show our custom prompt instead.
      this.$on("canInstall", (event) => {
        // Prevent Chrome 67 and earlier from automatically showing the prompt:
        event.preventDefault();

        // Stash the download event so it can be triggered later:
        this.deferredPrompt = event;
        this.modalOpen = true;
      });
    },
    methods: {
      text_title(id) {
        if (id === 0) return i18n.t("views_Home_upload");
        else if (id === 1) return i18n.t("views_Home_about");
        return i18n.t("translate_error");
      },
      
      // If the client accepts to install the PWA via our custom prompt, show the browser's install prompt
      promptInstall() {
        // Show the prompt:
        this.deferredPrompt.prompt();

        // Wait for the user to respond to the prompt:
        this.deferredPrompt.userChoice.then((choiceResult) => {
          if (choiceResult.outcome === "accepted") {
            console.log("User accepted the install prompt");
          } else {
            console.log("User dismissed the install prompt");
          }

          this.deferredPrompt = null;
          this.modalOpen = false;
        });
      },
    },
  };
</script>
