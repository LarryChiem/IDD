<template>
  <v-container 
    style="background: linear-gradient(to bottom right, #f5f7fa, #c3cfe2);"
    class="px-5 py-3 fill-height"
    fluid
  >
    <v-row justify="center">
      <v-col cols="6" md="6" sm="12">
        <v-card elevation="1" height="100%">
          <v-card-title class="headline purple darken-4 white--text justify-content-center" primary-title>
              Learn more
          </v-card-title>
          <v-card-content>
            <v-card-text class="subtitle-1">
                To learn more about the Multnomah County Intellectual and
                Developmental Disabilites department, you can visit our website.
            </v-card-text>
            <v-card-actions class="d-flex justify-content-center">
              <v-btn 
                color="purple darken-4"
                href="https://multco.us/dd"
                target="_blank"
                dark
              >
                  Learn More
              </v-btn>
            </v-card-actions>
          </v-card-content>
        </v-card>
      </v-col>

      <v-col cols="6" md="6" sm="12">
        <v-card elevation="1" height="100%">
          <v-card-title class="headline purple darken-2 white--text justify-content-center" primary-title>
              How to Install
          </v-card-title>
          <v-card-content>
            <v-card-text class="text-center title">
                To install this app onto your mobile device:
            </v-card-text>
            <v-card-text class="subtitle-1">
              <ol class="text-left">
                  <li>Open the settings of the browser</li>
                  <li>Click 'Add to Home screen'</li>
                  <li>Navigate to the home screen of your phone</li>
                  <li>Open the 'IDD App'</li>
                  <li>Installation completed</li>
              </ol>
            </v-card-text>
          </v-card-content>
        </v-card>
      </v-col>

      <v-col cols="12">
        <v-card elevation="1" height="100%">
          <v-card-title class="headline purple white--text justify-content-center" primary-title>
              Contributors
          </v-card-title>
          <v-card-content>
            <v-card-text  class="text-center title">
                    Built in Collaboration with the Portland State University Capstone Program and Multnomah County.
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
                        {{ student.position }}
                      </v-card-subtitle>
                    </v-card>
                  </v-col>
              </v-row>
            </v-container>
          </v-card-content>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
<script>
  export default {
    name: "about",
    data: function() {
      return {
        // A list of names and links for capstone students
        capstone_students: [
          {
            name: "Curtis Lewis", 
            link: "https://www.linkedin.com/in/curtis-james-lewis/",
            position: "Team Lead",
            color: "dark pink",
          },
          {
            name: "Larry Chiem", 
            link: "https://www.linkedin.com/in/larrychiem/",
            position: "Admin UI",
            color: "red",
          },
          {
            name: "John Brusaw", 
            link: "https://www.linkedin.com/in/johnbrusaw/",
            position: "Admin UI",
            color: "red",
          },
          {
            name: "Keaton Kraiger", 
            link: "https://www.linkedin.com/in/keaton-kraiger/",
            position: "Provider UI",
            color: "orange",
          },
          {
            name: "Yves Wienecke", 
            link: "https://www.linkedin.com/in/yves-wienecke/",
            position: "Provider UI",
            color: "orange",
          },
          {
            name: "David Post", 
            link: "https://www.linkedin.com/in/ohyonghao/",
            position: "Backend",
            color: "yellow darken-1",
          },
          {
            name: "Brian Lee", 
            link: "https://www.linkedin.com/in/brian-lee-pdx/",
            position: "Backend",
            color: "yellow darken-1",
          },
        ],
      };
    }
  };
</script>
