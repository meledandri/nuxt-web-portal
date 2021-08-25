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
          <v-dialog v-model="company.dialog" max-width="800px">
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
                        v-model="company.companyInfo.BusinessName"
                        label="BusinessName"
                        dense
                        outlined
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12">
                      <v-text-field
                        v-model="company.companyInfo.SRN"
                        label="SRN"
                        dense
                        outlined
                      ></v-text-field>
                    </v-col>
                    <v-col cols="12">
                      <v-combobox
                        v-model="companyRole"
                        :items="Roles"
                        item-value="companyRoleID"
                        item-text="companyRoleName"
                        :search-input.sync="company.search"
                        hint="Maximum of 5 tags"
                        label="Company Role"
                        return-object
                        auto-select-first
                        outlined
                        clearable
                      >
                        <template v-slot:no-data>
                          <v-list-item>
                            <v-list-item-content>
                              <v-list-item-title>
                                No results matching "<strong>{{
                                  search
                                }}</strong
                                >". Press <kbd>enter</kbd> to create a new one
                              </v-list-item-title>
                            </v-list-item-content>
                          </v-list-item>
                        </template>
                      </v-combobox>
                    </v-col>
                    <v-col cols="12">
                      <v-text-field
                        v-model="company.companyInfo.country"
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
                <v-btn color="blue darken-1" text @click="closeDCompany()">
                  Cancel
                </v-btn>
                <v-btn color="blue darken-1" text @click="saveCompany">
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
          <v-dialog v-model="user.dialog" max-width="800px">
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
                  <v-tabs v-model="user.tab" id="dUsers">
                    <v-tabs-slider></v-tabs-slider>

                    <v-tabs-items v-model="user.tab">
                      <v-tab-item>
                        <v-row>
                          <v-col cols="12" class="my-2">
                            <v-list two-line dense v-if="editedIndex > -1">
                              <v-list-item
                                v-for="user in company.companyInfo.details
                                  .users"
                                :key="user.userID"
                                dense
                              >
                                <v-list-item-avatar>
                                  <v-icon small>fa-user</v-icon>
                                </v-list-item-avatar>
                                <v-list-item-content>
                                  <v-list-item-title>
                                    {{ user.DisplayName }}
                                  </v-list-item-title>
                                  <v-list-item-subtitle>
                                    <b style="color: #4183a9;">{{
                                      user.UserName
                                    }}</b>
                                    ({{ user.email }})
                                  </v-list-item-subtitle>
                                </v-list-item-content>
                                <v-icon
                                  small
                                  class="mx-2 d-inline-block"
                                  @click="userReset(item)"
                                >
                                  fas fa-user-check
                                </v-icon>
                                <v-icon
                                  small
                                  color="red"
                                  class="mx-2 d-inline-block"
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
                              v-model="user.userInfo.UserName"
                              label="User Name"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-text-field
                              v-model="user.userInfo.DisplayName"
                              label="Display name"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-text-field
                              v-model="user.userInfo.email"
                              label="email"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-text-field
                              v-model="user.userInfo.password"
                              label="Temporary password"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-btn @click="user.tab = 0">Cancel</v-btn>
                            <v-btn @click="saveUser()">Save</v-btn>
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

          <!-- Dialog per la gestione delle attività -->
          <v-dialog v-model="task.dialog" max-width="800px">
            <v-card>
              <v-toolbar color="indigo" dark>
                <v-app-bar-nav-icon></v-app-bar-nav-icon>

                <v-toolbar-title>
                  {{ company.companyInfo.BusinessName }} \
                  Tasks</v-toolbar-title
                >

                <v-spacer></v-spacer>

                <v-btn icon>
                  <v-icon @click="newTask()">fa-plus</v-icon>
                </v-btn>
              </v-toolbar>

              <v-card-text>
                <v-container>
                  <v-tabs v-model="task.tab" id="dTasks">
                    <v-tabs-slider></v-tabs-slider>

                    <v-tabs-items v-model="task.tab">
                      <v-tab-item>
                        <v-row>
                          <v-col cols="12" class="my-2">
                            <v-list two-line dense v-if="editedIndex > -1">
                              <v-list-item
                                v-for="task in company.companyInfo.details
                                  .tasks"
                                :key="task.taskID"
                                dense
                              >
                                <v-list-item-avatar>
                                  <v-icon small>fas fa-thumbtack</v-icon>
                                </v-list-item-avatar>
                                <v-list-item-content>
                                  <v-list-item-title>
                                    {{ task.productName }} [{{
                                      task.mdClassName
                                    }}]
                                  </v-list-item-title>
                                  <v-list-item-subtitle>
                                    <b style="color: #4183a9;">{{
                                      task.mdActivityName
                                    }}</b>
                                    ({{ task.mdTaskStatesName }})
                                  </v-list-item-subtitle>
                                </v-list-item-content>

                                <v-icon small class="mx-2 d-inline-block">
                                  fas fa-user-check
                                </v-icon>
                                <v-icon
                                  small
                                  color="red"
                                  class="mx-2 d-inline-block"
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
                              v-model="task.taskInfo.productName"
                              label="Product Name"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12" md="6">
                            <v-combobox
                              v-model="computedClass"
                              :items="mdClass"
                              item-value="mdClassID"
                              item-text="mdClassName"
                              label="Class Name"
                              return-object
                              auto-select-first
                              outlined
                              clearable
                            >
                              <template v-slot:no-data>
                                <v-list-item>
                                  <v-list-item-content>
                                    <v-list-item-title>
                                      No results matching "<strong>{{
                                        search
                                      }}</strong
                                      >". Press <kbd>enter</kbd> to create a new
                                      one
                                    </v-list-item-title>
                                  </v-list-item-content>
                                </v-list-item>
                              </template>
                            </v-combobox>
                          </v-col>




                          <v-col cols="12" md="6">
                            <v-combobox
                              v-model="computedClass"
                              :items="mdClass"
                              item-value="mdClassID"
                              item-text="mdClassName"
                              label="Class Name"
                              return-object
                              auto-select-first
                              outlined
                              clearable
                            >
                              <template v-slot:no-data>
                                <v-list-item>
                                  <v-list-item-content>
                                    <v-list-item-title>
                                      No results matching "<strong>{{
                                        search
                                      }}</strong
                                      >". Press <kbd>enter</kbd> to create a new
                                      one
                                    </v-list-item-title>
                                  </v-list-item-content>
                                </v-list-item>
                              </template>
                            </v-combobox>
                          </v-col>










                          <v-col cols="12">
                            <v-text-field
                              v-model="user.userInfo.email"
                              label="email"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-text-field
                              v-model="user.userInfo.password"
                              label="Temporary password"
                              dense
                              outlined
                            ></v-text-field>
                          </v-col>

                          <v-col cols="12">
                            <v-btn @click="task.tab = 0">Cancel</v-btn>
                            <v-btn @click="saveTask()">Save</v-btn>
                          </v-col>
                        </v-row>
                      </v-tab-item>
                    </v-tabs-items>
                  </v-tabs>
                </v-container>
              </v-card-text>

              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" text @click="closeTasks">
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
        mdClass: [],
        mdActivity: [],
        Structures: [],
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
      user: {
        progress: false,
        tab: 0,
        dialog: false,
        userInfo: {
          userID: "",
          UserName: "",
          password: "",
          DisplayName: "",
          email: "",
          companyID: 0
        }
      },
      company: {
        progress: false,
        tab: 0,
        search: "",
        dialog: false,
        companyInfo: {
          companyID: "0",
          BusinessName: "",
          companyRoleID: "",
          companyRoleName: "",
          SRN: "",
          country: "",
          details: {}
        }
      },
      task: {
        progress: false,
        tab: 0,
        search: "",
        dialog: false,
        editedIndex: -1,
        taskInfo: {
          companyID: "0",
          BusinessName: "",
          productID: "0",
          productName: "",
          mdClassID: "0",
          mdClassName: "",
          editionID: "0",
          editionName: "",
          certificationPlan: "",
          mdActivityID: "0",
          mdActivityName: "",
          editionNotes: "",
          deadline: null,
          StructureID: "0",
          structureName: "",
          mdTaskID: "",
          mdTaskStatesID: "",
          mdTaskStatesName: "",
          insertDate: null,
          modDate: null,
          ownerID: "0",
          UserName: "",
          DisplayName: "",
          email: ""
        },
        taskDefault: {
          companyID: "0",
          BusinessName: "",
          productID: "0",
          productName: "",
          mdClassID: "0",
          mdClassName: "",
          editionID: "0",
          editionName: "",
          certificationPlan: "",
          mdActivityID: "0",
          mdActivityName: "",
          editionNotes: "",
          deadline: null,
          StructureID: "0",
          structureName: "",
          mdTaskID: "",
          mdTaskStatesID: "",
          mdTaskStatesName: "",
          insertDate: null,
          modDate: null,
          ownerID: "0",
          UserName: "",
          DisplayName: "",
          email: ""
        },
      }
    };
  },
  computed: {
    formTitle() {
      return this.editedIndex === -1 ? "New Item" : "Edit Item";
    },
    companyRole: {
      get() {
        var r = {
          companyRoleID:
            this.company.companyInfo.companyRoleID == undefined
              ? 0
              : this.company.companyInfo.companyRoleID,
          companyRoleName:
            this.company.companyInfo.companyRoleName == undefined
              ? ""
              : this.company.companyInfo.companyRoleName
        };
        return r;
      },
      set(v) {
        console.log("computed\\companyRole\\set");

        if (typeof v === "object" && v !== null) {
          this.company.companyInfo.companyRoleID = v.companyRoleID;
          this.company.companyInfo.companyRoleName = v.companyRoleName;
        } else {
          this.company.companyInfo.companyRoleID = 0;
          this.company.companyInfo.companyRoleName = v;
        }

        var r = {
          companyRoleID:
            this.company.companyInfo.companyRoleID == undefined
              ? 0
              : this.company.companyInfo.companyRoleID,
          companyRoleName:
            this.company.companyInfo.companyRoleName == undefined
              ? ""
              : this.company.companyInfo.companyRoleName
        };
        return r;
      }
    },
     computedClass: {
      get() {
        var r = {
          mdClassID:
            this.task.taskInfo.mdClassID == undefined
              ? 0
              : this.task.taskInfo.mdClassID,
          mdClassName:
            this.task.taskInfo.mdClassName == undefined
              ? ""
              : this.task.taskInfo.mdClassName
        };
        return r;
      },
      set(v) {
        console.log("computed\\companyRole\\set");

        if (typeof v === "object" && v !== null) {
          this.task.taskInfo.mdClassID = v.mdClassID;
          this.task.taskInfo.mdClassName = v.mdClassName;
        } else {
          this.task.taskInfo.mdClassID = 0;
          this.task.taskInfo.mdClassName = v;
        }

        var r = {
          mdClassID:
            this.task.taskInfo.mdClassID == undefined
              ? 0
              : this.task.taskInfo.mdClassID,
          mdClassName:
            this.task.taskInfo.mdClassName == undefined
              ? ""
              : this.task.taskInfo.mdClassName
        };
        return r;
      }
    },
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
      this.mdClass = data.mdClass;
      this.mdActivity = data.mdActivity;
      this.Structures = data.Structures;
      this.loadData = false;
    },
    fnNewUser() {
      console.log("fnNewUser");
      this.user.userInfo.userID = "0";
      this.user.userInfo.UserName = "";
      this.user.userInfo.password = "";
      this.user.userInfo.DisplayName = "";
      this.user.userInfo.email = "";
      this.user.userInfo.companyID = this.editedItem.companyID;
      this.user.tab = 1;
    },
    editItem(item) {
      console.log("editItem");
      this.editedIndex = this.Items.indexOf(item);
      this.company.companyInfo = Object.assign({}, item);
      this.company.dialog = true;
    },

    usersItem(item) {
      this.user.tab = 0;
      this.editedIndex = this.Items.indexOf(item);
      this.company.companyInfo = Object.assign({}, item);
      this.user.dialog = true;
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

    closeDCompany() {
      this.company.dialog = false;
      this.$nextTick(() => {
        //this.editedItem = Object.assign({}, this.defaultItem);
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
      this.user.dialog = false;
      this.$nextTick(() => {
        this.editedItem = Object.assign({}, this.defaultItem);
        this.editedIndex = -1;
      });
    },

    openTasks(item) {
      console.log("openTasks");
      this.editedIndex = this.Items.indexOf(item);
      this.company.companyInfo = Object.assign({}, item);
      this.task.dialog = true;
    },

    newTask() {
      console.log("newTask");
      this.editedIndex = 1;
      this.task.taskInfo = Object.assign({}, this.task.taskDefault);
      this.task.tab = 1;
      this.task.dialog = true;
    },

    editTask(item) {
      console.log("editTask");
      this.editedIndex = this.Items.indexOf(item);
      this.task.taskInfo = Object.assign({}, item);
      this.task.dialog = true;
    },

    closeTasks() {
      this.task.dialog = false;
      this.$nextTick(() => {
        this.task.taskInfo = Object.assign({}, this.task.taskDefault);
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
    },
    saveNewUser() {}
  }
};
</script>

<style>
#dUsers .v-tabs-bar,
#dTasks .v-tabs-bar {
  display: none;
}

#dUsers .v-list-item,
#dTasks .v-list-item {
  border-bottom: 1px dashed #95a2b1;
}
</style>
