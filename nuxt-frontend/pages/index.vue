<template>
<div>
  <div class="bg_wrapper" v-if="!configLoading">
  <div class="wrapper">
    <div class="container">
      <h1>{{ $t("welcome") }}</h1>
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
      <v-alert type="error" v-if="msg_err_login">
        {{ msg_err_login }}
      </v-alert>
      <v-row>
        <v-col cols="6">
          <nuxt-link
            class="mr-2"
            v-for="locale in availableLocales"
            :key="locale.code"
            :to="switchLocalePath(locale.code)"
            >{{ locale.name }}</nuxt-link
          >
        </v-col>
        <v-col cols="6">
          <v-btn color="primary" @click="loadConfig()" v-show="false"
            >Config</v-btn
          >
          <v-btn color="primary" @click="login()" :loading="loginProgess"
            >Login</v-btn
          >
        </v-col>
      </v-row>
      </v-form>
    </div>

  </div>
    <ul class="bg-bubbles">
      <li></li>
      <li></li>
      <li></li>
      <li></li>
      <li></li>
      <li></li>
      <li></li>
      <li></li>
      <li></li>
      <li></li>
    </ul>
  </div>
    <v-overlay opacity="0.75" color="primary" v-else>
      <v-progress-circular
        :size="100"
        color="white"
        indeterminate
      ></v-progress-circular>
    </v-overlay>
</div>
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
      msg_err_login: ""
    };
  },
  mounted() {},
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

<style lang="css" scoped>
@font-face {
  font-family: "Source Sans Pro";
  font-style: normal;
  font-weight: 200;
  src: url(https://fonts.gstatic.com/s/sourcesanspro/v14/6xKydSBYKcSV-LCoeQqfX1RYOo3i94_wlxdr.ttf)
    format("truetype");
}
@font-face {
  font-family: "Source Sans Pro";
  font-style: normal;
  font-weight: 300;
  src: url(https://fonts.gstatic.com/s/sourcesanspro/v14/6xKydSBYKcSV-LCoeQqfX1RYOo3ik4zwlxdr.ttf)
    format("truetype");
}
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
  font-weight: 300;
}
body {
  font-family: "Source Sans Pro", sans-serif;
  color: white;
  font-weight: 300;
}
body ::-webkit-input-placeholder {
  /* WebKit browsers */
  font-family: "Source Sans Pro", sans-serif;
  color: white;
  font-weight: 300;
}
body :-moz-placeholder {
  /* Mozilla Firefox 4 to 18 */
  font-family: "Source Sans Pro", sans-serif;
  color: white;
  opacity: 1;
  font-weight: 300;
}
body ::-moz-placeholder {
  /* Mozilla Firefox 19+ */
  font-family: "Source Sans Pro", sans-serif;
  color: white;
  opacity: 1;
  font-weight: 300;
}
body :-ms-input-placeholder {
  /* Internet Explorer 10+ */
  font-family: "Source Sans Pro", sans-serif;
  color: white;
  font-weight: 300;
}

.bg_wrapper {
    background: #50a3a2;
    background: linear-gradient(to bottom right, #50a3a2 0%, #53e3a6 100%);
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100vh;
    overflow: hidden;
}

.wrapper {
  background: rgba(255,255,255,0.5);
  position: absolute;
  top: 50%;
  left: 0;
  width: 100%;
  height: 400px;
  margin-top: -200px;
  overflow: hidden;
}
.wrapper.form-success .container h1 {
  transform: translateY(85px);
}
.container {
  max-width: 600px;
  margin: 0 auto;
  padding: 80px 0;
  height: 400px;
  text-align: center;
}
.container h1 {
  font-size: 40px;
  transition-duration: 1s;
  transition-timing-function: ease-in-put;
  font-weight: 200;
}
form {
  padding: 20px 0;
  position: relative;
  z-index: 2;
}
form input {
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  outline: 0;
  border: 1px solid rgba(255, 255, 255, 0.4);
  background-color: rgba(255, 255, 255, 0.2);
  width: 250px;
  border-radius: 3px;
  padding: 10px 15px;
  margin: 0 auto 10px auto;
  display: block;
  text-align: center;
  font-size: 18px;
  color: white;
  transition-duration: 0.25s;
  font-weight: 300;
}
form input:hover {
  background-color: rgba(255, 255, 255, 0.4);
}
form input:focus {
  background-color: white;
  width: 300px;
  color: #53e3a6;
}
form button {
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none;
  outline: 0;
  background-color: white;
  border: 0;
  padding: 10px 15px;
  color: #53e3a6;
  border-radius: 3px;
  width: 250px;
  cursor: pointer;
  font-size: 18px;
  transition-duration: 0.25s;
}
form button:hover {
  background-color: #f5f7f9;
}
.bg-bubbles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1;
}
.bg-bubbles li {
  position: absolute;
  list-style: none;
  display: block;
  width: 40px;
  height: 40px;
  background-color: rgba(255, 255, 255, 0.15);
  bottom: -160px;
  -webkit-animation: square 25s infinite;
  animation: square 25s infinite;
  transition-timing-function: linear;
}
.bg-bubbles li:nth-child(1) {
  left: 10%;
}
.bg-bubbles li:nth-child(2) {
  left: 20%;
  width: 80px;
  height: 80px;
  -webkit-animation-delay: 2s;
  animation-delay: 2s;
  -webkit-animation-duration: 17s;
  animation-duration: 17s;
}
.bg-bubbles li:nth-child(3) {
  left: 25%;
  -webkit-animation-delay: 4s;
  animation-delay: 4s;
}
.bg-bubbles li:nth-child(4) {
  left: 40%;
  width: 60px;
  height: 60px;
  -webkit-animation-duration: 22s;
  animation-duration: 22s;
  background-color: rgba(255, 255, 255, 0.25);
}
.bg-bubbles li:nth-child(5) {
  left: 70%;
}
.bg-bubbles li:nth-child(6) {
  left: 80%;
  width: 120px;
  height: 120px;
  -webkit-animation-delay: 3s;
  animation-delay: 3s;
  background-color: rgba(255, 255, 255, 0.2);
}
.bg-bubbles li:nth-child(7) {
  left: 32%;
  width: 160px;
  height: 160px;
  -webkit-animation-delay: 7s;
  animation-delay: 7s;
}
.bg-bubbles li:nth-child(8) {
  left: 55%;
  width: 20px;
  height: 20px;
  -webkit-animation-delay: 15s;
  animation-delay: 15s;
  -webkit-animation-duration: 40s;
  animation-duration: 40s;
}
.bg-bubbles li:nth-child(9) {
  left: 25%;
  width: 10px;
  height: 10px;
  -webkit-animation-delay: 2s;
  animation-delay: 2s;
  -webkit-animation-duration: 40s;
  animation-duration: 40s;
  background-color: rgba(255, 255, 255, 0.3);
}
.bg-bubbles li:nth-child(10) {
  left: 90%;
  width: 160px;
  height: 160px;
  -webkit-animation-delay: 11s;
  animation-delay: 11s;
}
@-webkit-keyframes square {
  0% {
    transform: translateY(0);
  }
  100% {
    transform: translateY(-1000px) rotate(600deg);
  }
}
@keyframes square {
  0% {
    transform: translateY(0);
  }
  100% {
    transform: translateY(-1000px) rotate(600deg);
  }
}
</style>
