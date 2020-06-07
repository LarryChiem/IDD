<template>
  <v-container 
    style="background: linear-gradient(to bottom right, #f5f7fa, #c3cfe2);"
    class="px-5 py-3 fill-height"
    fluid
  >
    <v-row justify="center">
      <v-col md="6" sm="12" xs="12">
        <v-card elevation="1" height="100%">
          <v-card-title class="headline purple darken-4 white--text justify-content-center" primary-title>
            {{ $t('views_About_learn') }}
          </v-card-title>
          <div>
            <v-card-text class="subtitle-1">
              {{ $t('views_About_learn_desc') }}
            </v-card-text>
            <v-card-actions class="d-flex justify-content-center">
              <v-btn 
                color="purple darken-4"
                href="https://multco.us/dd"
                target="_blank"
                dark
              >
                {{ $t('views_About_learn') }}
              </v-btn>
            </v-card-actions>
          </div>
        </v-card>
      </v-col>

      <v-col md="6" sm="12" xs="12">
        <v-card elevation="1" height="100%">
          <v-card-title class="headline purple darken-2 white--text justify-content-center" primary-title>
            {{ $t('views_About_install') }}
          </v-card-title>
          <div>
            <v-card-text class="text-center title">
              {{ $t('views_About_install_desc') }}
            </v-card-text>
            <v-card-text class="subtitle-1">
              <ol class="text-left">
                <li>{{ $t('views_About_install_li0') }}</li>
                <li>{{ $t('views_About_install_li1') }}</li>
                <li>{{ $t('views_About_install_li2') }}</li>
                <li>{{ $t('views_About_install_li3') }}</li>
                <li>{{ $t('views_About_install_li4') }}</li>
              </ol>
            </v-card-text>
          </div>
        </v-card>
      </v-col>

      <v-col cols="12">
        <v-card elevation="1" height="100%">
          <v-card-title class="headline purple white--text justify-content-center" primary-title>
            {{ $t('views_About_contributors') }}
          </v-card-title>
          <div>
            <v-card-text  class="text-center title">
              {{ $t('views_About_contributors_desc') }}
            </v-card-text>
            <v-container fluid>
              <v-row align="center" justify="center">
                  <v-col
                    cols="12"
                    md="3"
                    sm="8"
                    v-for="student in capstone_students"
                    :key="student.name"
                  >
                    <v-card 
                      class="mx-auto"
                      max-width="250"
                      dark
                      outlined
                      link
                      target="_blank"
                      :href="student.link"
                      :color="student.color"
                    >
                      <v-card-title>
                        {{ student.name }}
                      </v-card-title>
                      <v-card-subtitle>
                        {{ text_position(student.position) }}
                      </v-card-subtitle>
                    </v-card>
                  </v-col>
              </v-row>
            </v-container>
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
<script>
  import i18n from '@/plugins/i18n';
  
  export default {
    name: "about",
    data: function() {
      return {
        // A list of names and links for capstone students
        capstone_students: [
          {
            name: "Curtis Lewis", 
            link: "https://www.linkedin.com/in/curtis-james-lewis/",
            position: 0,
            color: "dark pink",
          },
          {
            name: "Larry Chiem", 
            link: "https://www.linkedin.com/in/larrychiem/",
            position: 1, 
            color: "red",
          },
          {
            name: "John Brusaw", 
            link: "https://www.linkedin.com/in/johnbrusaw/",
            position: 1,
            color: "red",
          },
          {
            name: "Keaton Kraiger", 
            link: "https://www.linkedin.com/in/keaton-kraiger/",
            position: 2,
            color: "orange",
          },
          {
            name: "Yves Wienecke", 
            link: "https://www.linkedin.com/in/yves-wienecke/",
            position: 2,
            color: "orange",
          },
          {
            name: "David Post", 
            link: "https://www.linkedin.com/in/ohyonghao/",
            position: 3,
            color: "yellow darken-1",
          },
          {
            name: "Brian Lee", 
            link: "https://www.linkedin.com/in/brian-lee-pdx/",
            position: 3,
            color: "yellow darken-1",
          },
        ],
      };
    },
    methods: {
      text_position(id) {
        if (id == 0) return i18n.t('views_About_contributors_position0');
        else if (id == 1) return i18n.t('views_About_contributors_position1');
        else if (id == 2) return i18n.t('views_About_contributors_position2');
        else if (id == 3) return i18n.t('views_About_contributors_position3');
        return i18n.t('translate_error');
      },
    },
  };
</script>
