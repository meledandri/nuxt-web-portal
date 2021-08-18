<template>
  <v-row>
    <v-col class="text-center">
      <img src="/v.png" alt="Vuetify.js" class="mb-5" />
      <blockquote class="blockquote">
        &#8220;Gestine Fabbricanti&#8221;
        <v-btn color="primary" @click="viewMessage()">Messaggio</v-btn>
        <footer>
          <small>
            <em>&mdash; {{ userInfo.DisplayName }}</em>
          </small>
        </footer>
      </blockquote>
    </v-col>
  </v-row>
</template>

<script>
export default {
  layout: "on",
  data() {
    return {
      content: "something..."
    };
  },
  methods: {
    viewMessage() {
      console.log("viewMessage..")
      this.$snotify.success("message", "title", {
        timeout: 2000,
        showProgressBar: true,
        closeOnClick: true
      });
    }
  }
};
</script>
