<template>
<v-card height="100%">
    <v-card-title>
      <v-text-field
        v-model="search"
        append-icon="fas fa-search"
        label="Search"
        single-line
        hide-details
      ></v-text-field>
    </v-card-title>
    <v-data-table
      :headers="headers"
      :items="Items"
      item-key="name"
      :search="search"
      :loading="loadData"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat>
          <v-toolbar-title
            ><v-icon color="primary" class="mr-2">fas fa-tasks</v-icon>
            Activities</v-toolbar-title
          >
          <v-divider class="mx-4" inset vertical></v-divider>
          <v-spacer></v-spacer>


        </v-toolbar>
      </template>
      <template v-slot:item.productName="{ item }">
        <v-icon small class="mr-2" color="primary" @click="usersItem(item)">
          fas fa-stethoscope
        </v-icon>        
        <span>{{item.productName}}</span> 
      </template>

      <template v-slot:item.actions="{ item }">
        <v-icon small class="mr-2" color="primary" @click="usersItem(item)">
          fas fa-users-cog
        </v-icon>
        <v-icon small class="mr-2" color="primary" @click="openTasks(item)">
          fas fa-tasks
        </v-icon>
        <v-icon small class="mr-2" @click="editItem(item)">
          fas fa-edit
        </v-icon>
        <v-icon small color="red" @click="deleteItem(item)">
          fas fa-trash-alt
        </v-icon>
      </template>
      <template v-slot:no-data>
        <v-btn color="primary" @click="loadDataList()">
          Reset
        </v-btn>
      </template>
    </v-data-table>
  </v-card>
</template>


<script>
export default {
  components: { },
  layout: "on",
  data() {
    return {
      loadData: false,
      search: "",
      headers: [
        { text: "Medical Decice", value: "productName" },
        { text: "Class", value: "mdClassName" },
        { text: "Type of activity", value: "mdActivityName" },
        { text: "Edition", value: "editionName" },
        { text: "State", value: "mdTaskStatesName" },
        { text: "Actions", value: "actions", sortable: false }
      ],
      Items: [],
      selectedCompany: 0
    };
  },
  computed: {
  },

  watch: {
  },
  mounted() {
    this.loadDataList();
  },
  methods: {
    async loadDataList() {
      console.log("loadDataList..");
      this.loadData = true;
      var data = (await this.$axios.get("activitiesList")).data;
      this.Items = data.list;
      this.loadData = false;
    },
  }
};
</script>