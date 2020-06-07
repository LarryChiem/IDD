<template>
  <v-app-bar app color="indigo" dark>
    <!-- Button to toggle the navigation drawer -->
    <v-app-bar-nav-icon @click.stop="onInput" />

    <!-- Application name -->
    <v-toolbar-title class="btn-indigo" @click="goHome()">
      {{ $t('components_AppShell_AppBar_title') }}
    </v-toolbar-title>

    <v-spacer></v-spacer>
      <div v-if="enableLanguageFeature">
        <label
          class="pa-0 ma-0 display-1" 
          @click="languageDrawerOpen = true" 
        >
          <flag
            style="border-radius: 5px;"
            :iso="languages[i18n.locale].flag" 
            :squared=false 
          /> 
        </label>
        <v-dialog v-model="languageDrawerOpen">
          <v-card>
            <v-card-title></v-card-title>
            <v-card-text>
              <v-radio-group v-model="i18n.locale" column>
                <v-radio 
                  v-for="(entry, key) in languages"
                  :key="key"
                  :label="entry.title" 
                  :value="entry.language"
                ></v-radio>
              </v-radio-group>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn 
                color="error" 
                @click="languageDrawerOpen = false"
                dark
              >
                Close
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </div>
  </v-app-bar>
</template>

<script>
  import i18n from '@/plugins/i18n';

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
        languages: {
          'en': { flag: 'us', language: 'en', title: 'English' },
          'ru': { flag: 'ru', language: 'ru', title: 'Русский' },
          'es': { flag: 'es', language: 'es', title: 'Español' },
          'zh-tw': { flag: 'tw', language: 'zh-tw', title: '繁體中文' }
        },
        languageDrawerOpen: false,
        i18n: i18n,
        enableLanguageFeature: false,
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
      goHome() {
        if(this.$router.history.current.path !== '/') {
          this.$router.push('/') 
        }
      },
    },
  };
</script>
