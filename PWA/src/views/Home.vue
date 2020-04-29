<template>
  <!-- Background image with a top->bottom gradient -->
  <v-img
    class="fill-height"
    gradient="to top, rgba(150,150,150,.3), rgba(255,255,255,.98)"
    max-height="100%"
    src="https://media.giphy.com/media/gg4BGWfydPAA0/giphy.gif"
  >
    <v-row align="center" justify="center">
      <v-col cols="12" md="4" sm="8">
        <v-img
          contain
          height="200"
          :src="require('../assets/icons/logo_full.svg')"
        />
      </v-col>
    </v-row>
  </v-img>
</template>

<script>
  export default {
    name: "Home",
    props: {
      source: String,
    },
  };
</script>
