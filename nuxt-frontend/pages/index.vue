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
                  prepend-icon="person"
                  name="login"
                  label="Login"
                  type="text"
                  v-model="usr"
                ></v-text-field>
                <v-text-field
                  id="password"
                  prepend-icon="lock"
                  name="password"
                  label="Password"
                  type="password"
                  v-model="pwd"
                ></v-text-field>
              </v-form>
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
      usr: "",
      pwd: "",
      loginProgess: false
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

    async login() {
      this.loginProgess = true;
      const params = {
        OldPassword: this.user_password_old,
        NewPassword: this.user_password_new,
        ConfirmPassword: this.user_password_new2
      };
      await this.$axios
        .post("login", params)
        .then(response => {
          this.loginProgess = false;
          var r = response.data;
          if (r.stato < 0) {
            // Errore login
          } else {
            this.userInfo = r.userInfo;
          }
        })
        .catch(e => {
          this.loginProgess = false;
          this.viewMessage("error", e.response.data.Message);
        });
    }
  }
};
</script>
