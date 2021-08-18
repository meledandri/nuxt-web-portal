<template>
  <v-app dark>
    <vue-snotify></vue-snotify>
    <v-main>
      <v-container>
        <Nuxt />
      </v-container>
    </v-main>
    <v-footer
      :absolute="!fixed"
      app
    >
      <span>Layouts\default - footer: &copy; {{ new Date().getFullYear() }}</span>
    </v-footer>
  </v-app>
</template>

<script>
import Vue from "vue"

import Snotify, { SnotifyPosition } from "vue-snotify";
const options = {
  toast: {
    position: SnotifyPosition.rightTop,
    timeout: 0,
    closeOnClick: true,
    pauseOnHover: true,
  },
};

Vue.use(Snotify, options);

import global from "~/mixins/global.js"
Vue.mixin(global)

export default {
  data () {
    return {
      clipped: false,
      drawer: false,
      fixed: false,
      items: [
        {
          icon: 'mdi-apps',
          title: 'Welcome',
          to: '/'
        },
        {
          icon: 'mdi-chart-bubble',
          title: 'Inspire',
          to: '/inspire'
        }
      ],
      miniVariant: false,
      right: true,
      rightDrawer: false,
      title: 'Vuetify.js'
    }
  },
    mounted: function() {
    this.loadConfig();
  },

}
</script>
