<template>
  <v-app dark>
    <vue-snotify></vue-snotify>

    <v-navigation-drawer
      v-model="drawer"
      clipped
      fixed
      app
      permanent
      expand-on-hover
    >
      <v-list dense>
        <v-list-item
          v-for="(item, i) in appMenu"
          :key="i"
          :to="item.Link"
          router
          exact
          dense
        >
          <v-list-item-action>
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title v-text="item.Name" />
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <v-app-bar clipped-left fixed dense app>
      <tfo-logo />
      <div class="lang-dropdown">
        <v-select
          :items="$i18n.locales"
          label="Lang"
          item-text="code"
          item-value="code"
          outlined
          dense
          class="mx-2"
          v-model="lng"
        >
        </v-select>
      </div>

      <v-spacer />

      <v-btn icon>
        <v-icon>far fa-bell</v-icon>
      </v-btn>
      <v-divider vertical class="mx-2" />
      <v-btn text>
        <v-icon class="mr-2" small>fas fa-people-carry</v-icon>
        Assistance
      </v-btn>
      <v-divider vertical class="mx-2" />

<v-btn icon to="/">
        <v-icon color="primary" small>fa-user</v-icon>

</v-btn>
      <v-divider vertical class="mx-2" />

      <v-btn icon @click.stop="rightDrawer = !rightDrawer">
        <v-icon>fa-menu</v-icon>
      </v-btn>
    </v-app-bar>
    <v-main>
      <v-container>
        <Nuxt />
      </v-container>
    </v-main>
    <v-navigation-drawer v-model="rightDrawer" :right="right" temporary fixed>
      <v-list>
        <v-list-item @click.native="right = !right">
          <v-list-item-action>
            <v-icon light>
              fas fa-redo-alt
            </v-icon>
          </v-list-item-action>
          <v-list-item-title>Switch drawer (click me)</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <v-footer :absolute="!fixed" app>
      <span>&copy; {{ new Date().getFullYear() }}</span>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  data() {
    return {
      clipped: false,
      drawer: false,
      fixed: false,
      items: [
        {
          icon: "fas fa-th",
          title: "Login",
          to: "/"
        },
        {
          icon: "fas fa-tachometer-alt",
          title: "Dashboard",
          to: "/dashboard_on"
        }
      ],
      miniVariant: false,
      right: true,
      rightDrawer: false,
      title: "ON - Dashboard"
    };
  },
  computed: {}
};
</script>

<style>
.lang-dropdown {
  max-width: 90px;
  max-height: 35px;
}
</style>
