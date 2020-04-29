<template>
  <v-app>
    <!-- Hideable navigation pane appearing on the left side of the webpage -->
    <NavigationDrawer
      :open="openNavigationDrawer"
      @drawer-change="handleDrawerChange"
    />

    <!-- Bar at the top of the webpage -->
    <AppBar :open="openNavigationDrawer" @drawer-change="handleDrawerChange" />

    <!-- Main content of the page, controlled by the Vue Router -->
    <v-content>
      <!-- Fade-in/Fade-out for smooth navigation transitions -->
      <transition mode="out-in" name="fade">
        <router-view />
      </transition>
    </v-content>

    <!-- Footer, appears at the bottom of the page -->
    <AppFooter />
  </v-app>
</template>

<script>
  import AppBar from "@/components/AppShell/AppBar";
  import AppFooter from "@/components/AppShell/AppFooter";
  import NavigationDrawer from "@/components/AppShell/NavigationDrawer";

  export default {
    name: "App",
    components: {
      AppBar,
      AppFooter,
      NavigationDrawer,
    },
    data: () => ({
      // Stores the value for if the navigation drawer is open or not
      openNavigationDrawer: false,
    }),
    methods: {
      // Toggle displaying the navigation drawer
      handleDrawerChange(isOpen) {
        this.openNavigationDrawer = isOpen;
      },
    },
  };
</script>

<style>
  .fade-enter-active,
  .fade-leave-active {
    transition-duration: 0.1s;
    transition-property: opacity;
    transition-timing-function: ease-out;
  }

  .fade-enter,
  .fade-leave-active {
    opacity: 0;
  }
</style>
