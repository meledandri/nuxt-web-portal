<template>
  <v-content>
    <v-container fluid fill-height v-if="!configLoading">
      <v-layout align-center justify-center>
        <v-flex xs12 sm8 md4>
          <v-card class="elevation-12">
            <v-toolbar dark color="primary">
              <v-toolbar-title>Login form</v-toolbar-title>
            </v-toolbar>
            <v-card-text>
              <v-form>
                <v-text-field
                  prepend-icon="fa-user"
                  name="login"
                  label="Login"
                  type="text"
                  v-model="usr"
                  autofocus
                  @keyup.enter="focusOnPassword()"
                ></v-text-field>
                <v-text-field
                  ref="pwd"
                  id="password"
                  prepend-icon="fa-lock"
                  name="password"
                  label="Password"
                  type="password"
                  v-model="pwd"
                  @keyup.enter="login()"
                ></v-text-field>
              </v-form>
              <v-alert type="error" v-if="msg_err_login">
      {{msg_err_login}}
    </v-alert>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="primary" @click="loadConfig()">Config</v-btn>
              <v-btn color="primary" @click="login()" :loading="loginProgess"
                >Login</v-btn
              >
            </v-card-actions>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
    <v-overlay opacity="0.75" color="primary" v-else>
      <v-progress-circular
        :size="100"
        color="white"
        indeterminate
      ></v-progress-circular>
    </v-overlay>
  </v-content>
</template>

<script>
export default {
  //   async asyncData({ $store, $config: { baseURL } }) {
  //     console.log('env:')
  //     console.log(process.env.NODE_ENV)
  //     console.log(process.env.BASE_URL)
  //     //console.log( $config.baseURL)
  //     var config = await fetch(`${baseURL}/appconfig`).then((res) => res.json())
  //     console.log(config)
  //     $store.commit("config/updateAppConfig", { Items: config });
  //     console.log($store)
  //     return { config }
  //   },
  //   components: {},
  //   data: () => ({
  //     item: {},
  // }),
  data() {
    return {
      usr: "admin",
      pwd: "Password123!",
      loginProgess: false,
      msg_err_login: "",
    };
  },
  methods: {
    // async loadConfig() {
    //   console.log("loadConfig");
    //   const vm = this
    //   var config = (await vm.$axios.get("appconfig")).data;
    //   for (var p in config) {
    //     this.appConfig[config[p].Parameter] = config[p].Value;
    //     console.log("Parametro acquisito: " + config[p].Parameter)
    //   }
    //var config = await fetch(`${baseURL}/appconfig`).then((res) => res.json())
    // }

    focusOnPassword() {
      const PasswordRef = this.$refs.pwd;
      PasswordRef.focus();
    }
  }
};
</script>
