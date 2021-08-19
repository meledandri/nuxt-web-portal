<template>
  <v-card>
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
      :single-expand="singleExpand"
      :expanded.sync="expanded"
      item-key="name"
      :search="search"
      :loading="loadData"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat>
          <v-toolbar-title
            ><v-icon color="primary" class="mr-2">far fa-building</v-icon>
            Fabbricanti</v-toolbar-title
          >
          <v-divider class="mx-4" inset vertical></v-divider>
          <v-spacer></v-spacer>
          <!-- Dialog per i dati dell'Azienda -->
          <v-dialog v-model="dialog" max-width="800px">
            <template v-slot:activator="{ on, attrs }">
              <v-btn color="primary" class="mb-2" v-bind="attrs" v-on="on" small
                ><v-icon color="success" class="mr-2" small>fa-plus</v-icon>
                Nuovo</v-btn
              >
            </template>
            <v-card>
              <v-card-title>
                <span class="text-h5">{{ formTitle }}</span>
              </v-card-title>

              <v-card-text>
                <v-container>
                  <v-row>
                    <v-col cols="12">
                      <v-text-field
                        v-model="editedItem.BusinessName"
                        label="BusinessName"
                        dense
                        outlined
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12">
                      <v-text-field
                        v-model="editedItem.SRN"
                        label="SRN"
                        dense
                        outlined
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12">
                      <v-select
                        v-model="editedItem.companyRoleID"
                        :items="Roles"
                        item-value="companyRoleID"
                        item-text="companyRoleName"
                        label="Company Role"
                        dense
                        outlined
                      ></v-select>
                    </v-col>
                    <v-col cols="12">
                      <v-text-field
                        v-model="editedItem.country"
                        label="Country"
                        dense
                        outlined
                      ></v-text-field>
                    </v-col>
                  </v-row>
                </v-container>
              </v-card-text>

              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" text @click="close">
                  Cancel
                </v-btn>
                <v-btn color="blue darken-1" text @click="save">
                  Save
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>

          <!-- Dialog per la cancellazione dell'Azienda -->
          <v-dialog v-model="dialogDelete" max-width="500px">
            <v-card>
              <v-card-title class="text-h5"
                >Are you sure you want to delete this item?</v-card-title
              >
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" text @click="closeDelete"
                  >Cancel</v-btn
                >
                <v-btn color="blue darken-1" text @click="deleteItemConfirm"
                  >OK</v-btn
                >
                <v-spacer></v-spacer>
              </v-card-actions>
            </v-card>
          </v-dialog>

          <!-- Dialog per la gestione degli utenti -->
          <v-dialog v-model="dialogUsers" max-width="800px">
            <v-card>
              <v-toolbar color="indigo" dark>
                <v-app-bar-nav-icon></v-app-bar-nav-icon>

                <v-toolbar-title>
                  {{ editedItem.BusinessName }} \ Users</v-toolbar-title
                >

                <v-spacer></v-spacer>

                <v-btn icon>
                  <v-icon @click="fnNewUser()">fa-plus</v-icon>
                </v-btn>
              </v-toolbar>

              <v-card-text>
                <v-container>
                  <v-tabs v-model="userTab" id="dUsers">
                    <v-tabs-slider></v-tabs-slider>

                    <v-tabs-items v-model="userTab">
                      <v-tab-item>
                        <v-row>
                          <v-col cols="12" class="my-2">
                            <v-list dense v-if="editedIndex > -1">
                              <v-list-item
                                v-for="user in editedItem.details.users"
                                :key="user.userID"
                                dense
                              >
                                <v-list-item-avatar>
                                  <v-icon small>fa-user</v-icon>
                                </v-list-item-avatar>
                                <v-list-item-content>
                                  <v-list-item-title>
                                    {{ user.DisplayName }} ({{ user.email }})
                                  </v-list-item-title>
                                </v-list-item-content>
                                <v-icon
                                  small
                                  class="mr-2 d-inline-block"
                                  @click="userReset(item)"
                                >
                                  fas fa-user-check
                                </v-icon>
                                <v-icon
                                  small
                                  color="red"
                                  class="mr-2 d-inline-block"
                                  @click="userDisable(item)"
                                >
                                  fas fa-user-slash
                                </v-icon>
                              </v-list-item>
                            </v-list>
                          </v-col>
                        </v-row>
                      </v-tab-item>

                      <v-tab-item>
                        <v-row class="ma-3">
                          <v-col cols="12">
                            <v-text-field
                              v-model="editedItem.country"
                              label="User Name"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-text-field
                              v-model="editedItem.country"
                              label="Display name"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-text-field
                              v-model="editedItem.country"
                              label="email"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-btn @click="userTab = 0">Cancel</v-btn>
                            <v-btn>Save</v-btn>
                          </v-col>
                        </v-row>
                      </v-tab-item>
                    </v-tabs-items>
                  </v-tabs>
                </v-container>
              </v-card-text>

              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" text @click="closeUsers">
                  Cancel
                </v-btn>
                <v-btn color="blue darken-1" text @click="save">
                  Save
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-dialog>
        </v-toolbar>
      </template>

      <template v-slot:item.actions="{ item }">
        <v-icon small class="mr-2" color="primary" @click="usersItem(item)">
          fas fa-users-cog
        </v-icon>
        <v-icon small class="mr-2" color="primary" @click="tasksItem(item)">
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
  layout: "on",
  data() {
    return {
      loadData: false,
      nHidden: 0,
      search: "",
      expanded: [],
      singleExpand: true,
      headers: [
        { text: "BusinessName", value: "BusinessName" },
        { text: "companyRoleName", value: "companyRoleName" },
        { text: "SRN", value: "SRN" },
        { text: "country", value: "country" },
        { text: "Actions", value: "actions", sortable: false }
      ],
      Items: [],
      Roles: [],
      dialog: false,
      dialogDelete: false,
      dialogUsers: false,
      dialogTasks: false,
      editedIndex: -1,
      editedItem: {
        companyID: 0,
        BusinessName: "",
        companyRoleID: 0,
        companyRoleName: "",
        SRN: "",
        country: ""
      },
      defaultItem: {
        companyID: 0,
        BusinessName: "",
        companyRoleID: 0,
        companyRoleName: "",
        SRN: "",
        country: ""
      },
      newUser: {
        userID: "",
        UserName: "",
        password: "",
        DisplayName: "",
        email: ""
      },
      userTab: 0
    };
  },
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    }
  },

  watch: {
    dialog(val) {
      val || this.close();
    },
    dialogDelete(val) {
      val || this.closeDelete();
    }
  },
  mounted() {
    this.loadDataList();
  },
  methods: {
    async loadDataList() {
      console.log("loadDataList..");
      this.loadData = true;
      var data = (await this.$axios.get("fablist")).data;
      this.nHidden = data.nHidden;
      this.Items = data.list;
      this.Roles = data.roles;
      this.loadData = false;
    },
    fnNewUser() {
      this.userID = "";
      this.UserName = "";
      this.password = "";
      this.DisplayName = "";
      this.email = "";
      this.userTab = 1;
    },
    editItem(item) {
      this.editedIndex = this.Items.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialog = true;
    },

    usersItem(item) {
      this.editedIndex = this.Items.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialogUsers = true;
    },

    deleteItem(item) {
      this.editedIndex = this.Items.indexOf(item);
      this.editedItem = Object.assign({}, item);
      this.dialogDelete = true;
    },

    deleteItemConfirm() {
      this.Items.splice(this.editedIndex, 1);
      this.closeDelete();
    },

    close() {
      this.dialog = false;
      this.$nextTick(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      });
    },

    closeDelete() {
      this.dialogDelete = false;
      this.$nextTick(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      });
    },

    closeUsers() {
      this.dialogUsers = false;
      this.$nextTick(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      });
    },

    save() {
      if (this.editedIndex > -1) {
        Object.assign(this.Items[this.editedIndex], this.editedItem);
      } else {
        this.Items.push(this.editedItem);
      }
      this.close();
    }
  }
};
</script>

<style >
#dUsers .v-tabs-bar {
  display: none;
}
</style>
