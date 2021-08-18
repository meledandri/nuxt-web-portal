<template>
  <v-row>
    <v-col class="text-center">
      <img
        src="/v.png"
        alt="Vuetify.js"
        class="mb-5"
      >
      <blockquote class="blockquote">
        &#8220;ACCESSO FABBRICANTE&#8221;
        <footer>
          <small>
            <em>&mdash; {{userInfo.DisplayName}}</em>
          </small>
        </footer>
      </blockquote>
    </v-col>
  </v-row>
</template>


<script>
export default {
  layout: 'fab',
  data() {
    return {
      content: 'something...'
    }
  }
}
</script>