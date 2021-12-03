<template>
    <span class="owner text-center" v-if="show">
      <v-icon dense color="primary" class="mr-2 float-left">
        fa fa-user
      </v-icon>
      {{item.displayName}}
    </span>
</template>
<script>
export default {
  name: "Owner",
  props: ["item", "product", "users"],
  data: () => ({}),
  computed: {
      show: {
          get(){return this.item.flagState > 0}
      },
    
  },
  methods: {
  },
};
</script>
<style scoped>
.owner
{
  min-width: 220px;
}
</style>