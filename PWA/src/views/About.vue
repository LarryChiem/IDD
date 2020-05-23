<template>
  <v-col>
    <v-card>
      <v-card-title class="headline blue white--text" primary-title>
        Learn more
      </v-card-title>

      <div class="text-center">
        <v-card-text>
          To learn more about the Multnomah County Intellectual and
          Developmental Disabilites department, you can visit our website.
        </v-card-text>
        <v-card-actions>
          <v-btn
            text
            color="deep-purple accent-4"
            href="https://multco.us/dd"
            target="_blank"
          >
            Learn More
          </v-btn>
        </v-card-actions>
      </div>
    </v-card>
  </v-col>
</template>

<script>
  export default {
    name: "about"
  };
</script>
