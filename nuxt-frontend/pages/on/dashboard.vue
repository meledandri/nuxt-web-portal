<template>
  <v-row>
    <v-col class="text-center">
      <img
        src="/tfo-logo.png"
        alt="Vuetify.js"
        class="mb-5"
      >
      <blockquote class="blockquote">
        &#8220;ACCESSO ORGANISMO NOTIFICATO&#8221;
        <footer>
          <small>
            <em>&mdash; {{userInfo.DisplayName}} ({{ $t('welcome') }})</em>
          </small>
        </footer>
      </blockquote>
    </v-col>
  </v-row>
</template>


<script>
export default {
  layout: 'on',
  data() {
    return {
      content: 'something...'
    }
  }
}
</script>