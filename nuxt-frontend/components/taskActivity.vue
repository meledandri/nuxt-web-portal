<template>
  <v-tabs v-model="task.tab" id="dTasks">
    <v-tabs-slider></v-tabs-slider>

    <v-tabs-items v-model="task.tab">
      <!--                            Tab dell'elenco delle attività -->
      <v-tab-item>
        <v-row>
          <v-col cols="12" class="my-2">
            <v-list two-line dense v-if="selectedCompany > 0">
              <v-list-item
                v-for="task in company.companyInfo.details.tasks"
                :key="task.taskID"
                dense
              >
                <v-list-item-avatar>
                  <v-icon small>fas fa-thumbtack</v-icon>
                </v-list-item-avatar>
                <v-list-item-content>
                  <v-list-item-title>
                    {{ task.productName }} [{{ task.mdClassName }}]
                  </v-list-item-title>
                  <v-list-item-subtitle>
                    <b style="color: #4183a9;">{{ task.mdActivityName }}</b>
                    ({{ task.mdTaskStatesName }})
                  </v-list-item-subtitle>
                </v-list-item-content>

                <v-icon small class="mx-2 d-inline-block">
                  far fa-edit
                </v-icon>
                <v-icon small color="red" class="mx-2 d-inline-block">
                  far fa-trash-alt
                </v-icon>
              </v-list-item>
            </v-list>
          </v-col>
        </v-row>
      </v-tab-item>
      <!--                            Tab di modifica o inserimento attività -->
      <v-tab-item>
        <v-row class="ma-3">
          <!-- Nome Prodotto -->
          <v-col cols="12">
            <v-combobox
              v-model="computedProduct"
              :items="products"
              item-value="productID"
              item-text="productName"
              label="Product Name"
              return-object
              auto-select-first
              outlined
              clearable
            >
              <template v-slot:no-data>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>
                      No results matching "<strong>{{ search }}</strong
                      >". Press <kbd>enter</kbd> to create a new one
                    </v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </template>
            </v-combobox>
          </v-col>
          <!-- Classe Prodotto -->
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
                      No results matching "<strong>{{ search }}</strong
                      >". Press <kbd>enter</kbd> to create a new one
                    </v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </template>
            </v-combobox>
          </v-col>
          <!-- Attività possibili sul prodotto -->
          <v-col cols="12" md="6">
            <v-combobox
              v-model="computedmdActivity"
              :items="mdActivity"
              item-value="mdActivityID"
              item-text="mdActivityName"
              label="Activity Name"
              return-object
              auto-select-first
              outlined
              clearable
            >
              <template v-slot:no-data>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title>
                      No results matching "<strong>{{ search }}</strong
                      >". Press <kbd>enter</kbd> to create a new one
                    </v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </template>
            </v-combobox>
          </v-col>
          <!-- Codice del Prodotto -->
          <v-col cols="12" md="6">
            <v-text-field label="Product code" outlined></v-text-field>
          </v-col>
          <!-- Edizione Prodotto -->
          <v-col cols="12" md="6">
            <v-text-field
              label="Product Edition"
              v-model="computedEdition"
              outlined
            ></v-text-field>
          </v-col>

          <!-- Piano di certificazione -->
          <v-col cols="12" md="6">
            <v-text-field
              label="Certification plan"
              v-model="task.taskInfo.certificationPlan"
              outlined
            ></v-text-field>
          </v-col>

          <!-- Tipo archivio -->
          <v-col cols="12" md="6">
            <v-select
              :items="Structures"
              label="Upload mode"
              item-text="structureName"
              item-value="structureID"
              v-model="computedStructure"
              return-object
              outlined
            ></v-select>
          </v-col>

          <!-- Note sull'edizione -->
          <v-col cols="12">
            <v-textarea
              outlined
              label="Notes"
              :value="task.taskInfo.editionNotes"
            ></v-textarea>
          </v-col>

          <v-col cols="12">
            <v-btn @click="task.tab = 0">Cancel</v-btn>
            <v-btn @click="saveTask()">Save</v-btn>
          </v-col>
        </v-row>
      </v-tab-item>
    </v-tabs-items>
  </v-tabs>
</template>

<script>
export default {
  //Definizione-----------------
  name: "component-activity",
  //   components: {},        //Elenco di componenti utilizzati
  props: ["scompany"],
  data: () => ({
    loadData: false,
    Roles: [],
    mdClass: [],
    mdActivity: [],
    Structures: [],
    products: [],
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
        details: { users: [], tasks: [] }
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
      }
    }
  }), // i dati definiscono un oggetto che rappresenta i dati interni del componente Vue. Può anche essere una funzione che restituisce l'oggetto dati.
  methods: {
    async loadCompanyData(companyID) {
      console.log("loadCompanyData..(" + companyID + ")");
      this.loadData = true;
      var data = (await this.$axios.get("fabEditData/" + companyID)).data;
      this.company.companyInfo = data.company;
      this.products = data.products;
      this.Roles = data.roles;
      this.mdClass = data.mdClass;
      this.mdActivity = data.mdActivity;
      this.Structures = data.Structures;
      this.loadData = false;
    },
    changeTab(n) {
      this.task.tab = n;
    }
  }, //l'oggetto metodi contiene una coppia chiave-valore di nomi di metodo e la relativa definizione di funzione. Questi fanno parte del comportamento del componente Vue che l'altro componente può attivare.
  computed: {
    tab() {
      return this.$attrs.tab;
    },
    user() {
      return this.$attrs.user;
    },
    editedIndex() {
      return this.$attrs.editedIndex;
    },
    // Roles() {
    //   return this.$attrs.Roles;
    // },
    // mdClass() {
    //   return this.$attrs.mdClass;
    // },
    // mdActivity() {
    //   return this.$attrs.mdActivity;
    // },
    // Structures() {
    //   return this.$attrs.Structures;
    // },
    selectedCompany() {
      return this.$attrs.selectedCompany;
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
    computedProduct: {
      get() {
        var r = {
          productID:
            this.task.taskInfo.productID == undefined
              ? 0
              : this.task.taskInfo.productID,
          productName:
            this.task.taskInfo.productName == undefined
              ? ""
              : this.task.taskInfo.productName
        };
        return r;
      },
      set(v) {
        console.log("computed\\companyRole\\set");

        if (typeof v === "object" && v !== null) {
          this.task.taskInfo.productID = v.productID;
          this.task.taskInfo.productName = v.productName;
        } else {
          this.task.taskInfo.productID = 0;
          this.task.taskInfo.productName = v;
        }

        var r = {
          productID:
            this.task.taskInfo.productID == undefined
              ? 0
              : this.task.taskInfo.productID,
          productName:
            this.task.taskInfo.productName == undefined
              ? ""
              : this.task.taskInfo.productName
        };
        return r;
      }
    },
    computedmdActivity: {
      get() {
        var r = {
          mdActivityID:
            this.task.taskInfo.mdActivityID == undefined
              ? 0
              : this.task.taskInfo.mdActivityID,
          mdActivityName:
            this.task.taskInfo.mdActivityName == undefined
              ? ""
              : this.task.taskInfo.mdActivityName
        };
        return r;
      },
      set(v) {
        console.log("computed\\companyRole\\set");

        if (typeof v === "object" && v !== null) {
          this.task.taskInfo.mdActivityID = v.mdActivityID;
          this.task.taskInfo.mdActivityName = v.mdActivityName;
        } else {
          this.task.taskInfo.mdActivityID = 0;
          this.task.taskInfo.mdActivityName = v;
        }

        var r = {
          mdActivityID:
            this.task.taskInfo.mdActivityID == undefined
              ? 0
              : this.task.taskInfo.mdActivityID,
          mdActivityName:
            this.task.taskInfo.mdActivityName == undefined
              ? ""
              : this.task.taskInfo.mdActivityName
        };
        return r;
      }
    },
    computedEdition: {
      get() {
        return this.task.taskInfo.editionName == undefined
          ? ""
          : this.task.taskInfo.editionName;
      },
      set(v) {
        this.task.taskInfo.editionID = 0;
        this.task.taskInfo.editionName = v;

        return this.task.taskInfo.editionName == undefined
          ? ""
          : this.task.taskInfo.editionName;
      }
    },
    computedStructure: {
      get() {
        var r = {
          structureID:
            this.task.taskInfo.structureID == undefined
              ? 0
              : this.task.taskInfo.structureID,
          structureName:
            this.task.taskInfo.structureName == undefined
              ? ""
              : this.task.taskInfo.structureName
        };
        return r;
      },
      set(v) {
        console.log("computed\\companyRole\\set");

        if (typeof v === "object" && v !== null) {
          this.task.taskInfo.structureID = v.structureID;
          this.task.taskInfo.structureName = v.structureName;
        } else {
          this.task.taskInfo.structureID = 0;
          this.task.taskInfo.structureName = v;
        }

        var r = {
          structureID:
            this.task.taskInfo.structureID == undefined
              ? 0
              : this.task.taskInfo.structureID,
          structureName:
            this.task.taskInfo.structureName == undefined
              ? ""
              : this.task.taskInfo.structureName
        };
        return r;
      }
    }
  }, // contiene un oggetto che definisce le funzioni getter e setter per le proprietà calcolate del componente Vue. Le proprietà calcolate influenzano un aggiornamento reattivo sul DOM ogni volta che il loro valore cambia.
  props: {}, //contiene un array o un oggetto di proprietà specifiche del componente Vue.js, impostato al momento dell'invocazione.
  watch: {
    "task.tab": function(n) {
      console.log("watch task.tab..");
      if (n == 0) {
        this.loadCompanyData(this.selectedCompany);
      }
    },
    selectedCompany(n) {
      if (n != 0) {
        this.loadCompanyData(this.selectedCompany);
      }
    }
  }, // questo oggetto tiene traccia dei cambiamenti nel valore di una qualsiasi delle proprietà definite come parte dei "dati" impostando le funzioni per controllarli.
  //Eventi------------------------
  beforeCreate() {}, //questo è il primo stato del ciclo di vita. Non puoi ancora interagire con nessuna parte del componente
  created() {
    this.loadCompanyData(this.selectedCompany);
  }, //questo è subito dopo la creazione dell'istanza del componente. Ora puoi interagire con il componente, ad es. le proprietà dei dati, i watcher, le proprietà calcolate, ma non puoi ancora accedere al DOM. Di solito, i dati vengono recuperati dal database o dall'API in questo hook del ciclo di vita.
  beforeMount() {}, //il componente è compilato in questa fase, ma deve ancora essere visualizzato sullo schermo.
  mounted() {}, //questo avviene dopo che il componente è stato montato. Ora puoi accedere al metodo $ el e giocare con il contenuto all'interno degli elementi HTML. In questa fase il componente diventa completamente interattivo.
  beforeUpdate() {}, //ogni volta che vengono apportate modifiche ai dati o al DOM, subito prima, viene chiamato questo hook del ciclo di vita. Ciò è utile quando è necessario registrare le modifiche.
  updated() {}, //subito dopo che sono state apportate le modifiche al DOM o ai dati. Qui puoi eseguire operazioni dipendenti dalla modifica nel DOM.
  beforeDestroy() {}, //questo è subito prima che il componente venga distrutto ed è l'ultima istanza del DOM completamente funzionante. Puoi eseguire le operazioni di chiusura necessarie.
  destroyed() {} //questo è un po 'simile all'hook beforeCreate in cui il componente non è funzionale e non è possibile accedere a proprietà dei dati, watcher, proprietà calcolate ed eventi.
};
</script>


<style scoped>
.v-input:not(.v-textarea)
{
    max-height: 50px;
}

</style>