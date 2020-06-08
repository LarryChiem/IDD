<template>
  <v-container class="red lighten-5" fill-height fluid>
    <v-row align="center" class="ma-12" justify="center">
      <v-col>
        <v-alert type="error"> 404 - {{ $t("views_NotFound_text") }} </v-alert>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
  export default {
    name: "NotFound",
  };
</script>
