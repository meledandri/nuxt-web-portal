<template>
  <v-row>
    <v-col class="text-center" cols="12">
      <img
        src="/tfo-logo.png"
        alt="Vuetify.js"
        class="mb-5"
        width="200px"
      >
      <blockquote class="blockquote">
        &#8220;ACCESSO ORGANISMO NOTIFICATO&#8221;
        <footer>
          <small>
            <em>&mdash; {{userInfo.displayName}} ({{ $t('welcome') }})</em>
          </small>
        </footer>
      </blockquote>
    </v-col>
    <v-col cols="4">
<v-card
    class="mx-auto text-center"
    color="green"
    dark
    max-width="600"
  >
    <v-card-text>
      <v-sheet color="rgba(0, 0, 0, .12)">
        <v-sparkline
          :value="value"
          color="rgba(255, 255, 255, .7)"
          height="100"
          padding="24"
          stroke-linecap="round"
          smooth
        >
          <template v-slot:label="item">
            ${{ item.value }}
          </template>
        </v-sparkline>
      </v-sheet>
    </v-card-text>

    <v-card-text>
      <div class="text-h4 font-weight-thin">
        Sales Last 24h
      </div>
    </v-card-text>

    <v-divider></v-divider>

    <v-card-actions class="justify-center">
      <v-btn
        block
        text
      >
        Go to Report
      </v-btn>
    </v-card-actions>
  </v-card>
    </v-col>

<v-divider />

    <v-col cols="4">
<v-card
    class="mt-4 mx-auto"
    max-width="400"
  >
    <v-sheet
      class="v-sheet--offset mx-auto"
      color="cyan"
      elevation="12"
      max-width="calc(100% - 32px)"
    >
      <v-sparkline
        :labels="labels2"
        :value="value2"
        color="white"
        line-width="2"
        padding="16"
      ></v-sparkline>
    </v-sheet>

    <v-card-text class="pt-0">
      <div class="text-h6 font-weight-light mb-2">
        User Registrations
      </div>
      <div class="subheading font-weight-light grey--text">
        Last Campaign Performance
      </div>
      <v-divider class="my-2"></v-divider>
      <v-icon
        class="mr-2"
        small
      >
        mdi-clock
      </v-icon>
      <span class="text-caption grey--text font-weight-light">last registration 26 minutes ago</span>
    </v-card-text>
  </v-card>
    </v-col>
    <v-col cols="4">

    </v-col>
  </v-row>
</template>


<script>
export default {
  layout: 'on',
  data() {
    return {
      content: 'something...',
      value: [
        423,
        446,
        675,
        510,
        590,
        610,
        760,
      ],
      labels2: [
        '12am',
        '3am',
        '6am',
        '9am',
        '12pm',
        '3pm',
        '6pm',
        '9pm',
      ],
      value2: [
        200,
        675,
        410,
        390,
        310,
        460,
        250,
        240,
      ],
    }
  }
}
</script>