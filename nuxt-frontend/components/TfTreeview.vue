<template>
  <div class="treeview-content">
    <h1 :color="primary">{{product.productName}} ({{product.structureName}})</h1>

    <!-- Visualizzazioe ad albero dei documenti -->
    <v-treeview
      :open.sync="open"
      :items="items"
      :search="search"
      item-key="detailID"
      open-on-click
      overlay-color="#26639F"
      transition
      loading-icon="loading_details_list"
      id="detail-list"
      ref="detailList"
      activatable
      v-if="product && items.length && !loadData"
    >
      <template v-slot:prepend="{ item, open }">
        <v-icon
          v-if="
            (item.children && item.children.length) || item.flagContainer == 2
          "
          color="#E7BD48"
        >
          {{ open ? "fas fa-folder-open" : "fas fa-folder" }}
        </v-icon>

        <v-progress-circular
          v-if="processingDocument(item.id) && item.flagContainer == 1"
          indeterminate
          color="primary"
        ></v-progress-circular>
        <v-icon
          v-else-if="item.flagContainer == 1 && item.flagState > 0"
          :color="
            item.fileExtension.trim() == 'pdf'
              ? '#B71C1C'
              : item.flagState > 0 && item.fileExtension.trim() == 'docx'
              ? '#295391'
              : item.flagState > 0 && item.fileExtension.trim() == 'xlsx'
              ? '#1C6D42'
              : '#BDBDBD'
          "
        >
          {{ files[item.fileExtension.trim()] == undefined ? 'far fa-file-alt' : files[item.fileExtension.trim()]}}
        </v-icon>
        <v-icon
          v-else-if="item.flagContainer == 1 && item.flagState == 0"
          :color="'#BDBDBD'"
          >far fa-file</v-icon
        >
      </template>
      <template v-slot:label="{ item }">
        <!-- Label titolo paragrafo -->
        <!-- Label paragrafo -->
        <a
          @click="
            item.flagContainer == 1
              ? setAction('editDocument', item)
              : setAction('editFolder', item)
          "
          style=""
          :class="
            item.flagState == 1 && userCanChange(item)
              ? 'red--text'
              : item.flagContainer == 0 &&
                !(item.children && item.children.length)
              ? 'label-title'
              : item.flagContainer == 0
              ? 'black--text'
              : 'primary--text'
          "
          >{{ item.Title }}</a
        >
      </template>

      <template v-slot:append="{ item }">
        <v-progress-linear
          v-if="processingDocument(item.id) && item.flagContainer == 1"
          active
          color="deep-purple accent-4"
          indeterminate
          rounded
          height="6"
          class="d-block"
          style="width: 200px;"
        ></v-progress-linear>

        <div v-else>
          <!-- Azioni per i file -->
          <v-list-item v-if="item.flagContainer == 1" class="px-2">
            <!-- Gestione STATO -->

            <v-menu
              close-on-click
              close-on-content-click
              offset-x
              offset-y
              :ref="'cm' + item.id"
            >
              <template v-slot:activator="{ on, attrs }">
                <v-btn
                  x-small
                  :dark="item.flagState != 0"
                  :outlined="item.flagState == 0"
                  v-bind="attrs"
                  v-on="on"
                  class="btn-tab"
                >
                  {{ getStateName(item.flagState) }}
                  <v-icon right x-small>fas fa-chevron-down</v-icon>
                </v-btn>
              </template>

              <v-list dense>
                <v-list-item
                  v-for="(citem, index) in limitOptions(item)"
                  :key="index"
                  @click="setAction(citem.action, item)"
                  :class="citem.id == 0 ? 'my-0' : ''"
                >
                  <!-- Altri tasti -->
                  <v-list-item-icon
                    ><v-icon small :color="citem.iconColor">{{
                      citem.icon
                    }}</v-icon></v-list-item-icon
                  >
                  <v-list-item-title>{{ citem.text }}</v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
            <v-divider class="mx-2" vertical>--</v-divider>
            <input
              type="file"
              :ref="'xupload' + item.id"
              :accept="uploadAcceptType"
              @change="onFilePicked"
              :disabled="product.flagState != 1"
              style="display:none;"
              v-if="permitted('tf_edit_document')"
            />
            <v-file-input
              v-model="uploadFiles"
              :rules="uploadRules"
              :accept="uploadAcceptType"
              hide-input
              color="primary"
              v-if="
                permitted('tf_edit_document') &&
                  (item.flagState == 0 || item.flagState == 8) &&
                  product.flagState == 1 &&
                  product.Documentazione !== 99
              "
              :class="'py-0 my-0' + (item.flagState == 8 ? ' hidden' : '')"
              style="width:36px"
              prepend-inner-icon="fas fa-upload"
              @click="resetAttachFile('upload' + item.id)"
              @change="attachFile('upload' + item.id, item)"
              :id="'upload' + item.id"
              :ref="'upload' + item.id"
              @test="$emit('click')"
            >
            </v-file-input>
            <v-icon
              v-if="
                permitted('tf_edit_document') &&
                  (item.flagState == 0 || item.flagState == 8) &&
                  product.flagState != 1
              "
              class="mx-2"
              color="#cecece"
              >fas fa-paperclip</v-icon
            >
            <!-- Pulsante Note -->
            <v-btn
              icon
              color="primary"
              v-if="
                item.flagState != 0 &&
                  product.Documentazione !== 99 &&
                  permitted('tf_edit_document')
              "
              @click="setAction('addDocumentNotes', item)"
            >
              <v-badge
                bordered
                overlap
                :content="item.Notes"
                :value="item.Notes > 0"
                color="primary"
              >
                <v-icon
                  v-text="
                    item.Notes == 0
                      ? 'far fa-sticky-note'
                      : 'fas fa-sticky-note'
                  "
                  :color="
                    item.Ext_id == -1
                      ? 'red'
                      : item.Notes == 0
                      ? 'primary'
                      : 'amber'
                  "
                />
              </v-badge>
            </v-btn>
          </v-list-item>

          <!-- Azioni per le cartelle -->
          <v-list-item
            v-if="
              permitted('tf_edit_document') &&
                ((item.children && item.children.length) ||
                  item.flagContainer == 2)
            "
            class="px-2"
          >
            <v-menu
              close-on-click
              close-on-content-click
              offset-x
              offset-y
              allow-overflow
            >
              <template v-slot:activator="{ on, attrs }">
                <v-btn
                  x-small
                  :dark="item.flagState != 0"
                  :outlined="item.flagState == 0"
                  v-bind="attrs"
                  v-on="on"
                  v-if="item.flagContainer == 2"
                  class="btn-tab"
                >
                  {{ $t("Aggiungi") }}
                  <v-icon right x-small v-if="product.flagState == 1"
                    >fas fa-chevron-down</v-icon
                  >
                </v-btn>
                <v-btn
                  x-small
                  :dark="item.flagState != 0"
                  :outlined="item.flagState == 0"
                  v-bind="attrs"
                  v-on="on"
                  v-if="item.flagContainer == 2"
                  class="btn-tab"
                >
                  {{ $t("Opzioni") }}
                  <v-icon right x-small v-if="product.flagState == 1"
                    >fas fa-chevron-down</v-icon
                  >
                </v-btn>
              </template>
              <v-list dense v-if="item.flagState == 0">
                <v-list-item
                  v-for="(citem, index) in limitOptions(item)"
                  :key="index"
                  @click="setAction(citem.action, item)"
                >
                  <v-list-item-icon
                    ><v-icon small :color="citem.iconColor">{{
                      citem.icon
                    }}</v-icon></v-list-item-icon
                  >
                  <v-list-item-title>{{ citem.text }}</v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
            <v-divider class="mx-2" vertical>--</v-divider>
            <v-btn icon color="secondary">
              <v-icon v-if="item.flagContainer == 2">fas fa-folder-plus</v-icon>
              <v-icon v-else>far fa-folder</v-icon>
            </v-btn>
          </v-list-item>
        </div>
      </template>
    </v-treeview>
  </div>
</template>

<script>
export default {
  //Definizione-----------------
  name: "component-TF-treeview",
  //   components: {},        //Elenco di componenti utilizzati
  data: () => ({
    loadData: false,
    items: [],
    product: {},
    open: [],
    search: "",
    inProgress: []
  }), // i dati definiscono un oggetto che rappresenta i dati interni del componente Vue. Può anche essere una funzione che restituisce l'oggetto dati.
  methods: {
    async loadDataList() {
      console.log("loadDataList..");
      this.loadData = true;
      var data = (await this.$axios.get("tree/" + this.editionID)).data;
      this.items = data.tree;
      if (this.items.length) {
        this.product = this.items[0];
      }
      this.loadData = false;
    },
    startProcess(id) {
      //Aggiunge l'id dai processi in lavorazione..
      const index = this.inProgress.indexOf(id);
      if (index == -1) {
        this.inProgress.push(id);
        setTimeout(() => this.endProcess(id), 30000);
      }
    },
    endProcess(id) {
      //Rimuove l'id dai processi in lavorazione..
      const index = this.inProgress.indexOf(id);
      if (index > -1) {
        this.inProgress.splice(index, 1);
      }
    },
    processingDocument(id) {
      return this.inProgress.indexOf(id) > -1;
    },
    limitOptions(item) {
      // Crea un elenco delle voci contestuali in base allo stato
      var options = this.detail_options;
      var o = [];
      if (options && options.length) {
        // if (item.IDutente !== 0 && item.IDutente !== this.idUtente) {
        //   return [
        //     {
        //       id: 10000,
        //       text: "Prendi in gestione",
        //       icon: "fas fa-ban",
        //       action: "setDocumentToAuthor",
        //       detail_states: [1],
        //     },
        //   ];
        // }
        for (var opt in options) {
          var state = item.flagState;
          if (options[opt].detail_states.indexOf(state) !== -1) {
            var check = false;
            var fc = item.flag_contenitore;
            var idd = item.detail_id;
            var AddFolder = item.AddFolder;
            var AddFile = item.AddFile;
            var NLivelli = item.NLivelli;
            var idVerDoc = item.ver;

            switch (
              options[opt].id //In base all'azione richiesta possiamo filtrare maggiormente in base ai dati del documento
            ) {
              case 1009: //Modifica titolo
              // check = item.detail_id > 1000000000000000; // Solo se idDocumento > 1000000000000000
              // if (check) {
              //   o.push(options[opt]);
              // }
              // break;

              default:
                o.push(options[opt]);
                console.log("detailsList\\methods\\limitOptions (default)");
            }
          }
        }
        return o;
      }
      console.log(item);
      return null;
    },
    userCanChange(item) {
      return (
        (item.flagState !== 0 &&
          item.IDutente !== 0 &&
          item.IDutente == this.idUtente) ||
        item.flagState == 2
      );
    },
    getStateName(val) {
      //Recupera il nome dello stato in base al codice
      return this.detail_states[val];
    },
    onFilePicked(event) {
      this.uploadFiles = event.target.files[0];
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
    selectedCompany() {
      return this.$attrs.selectedCompany;
    },
    editionID() {
      return this.$attrs.editionID;
    },
    detail_states() {
      return {
        0: this.$t("DaGestire"),
        1: this.$t("InGestione"),
        2: this.$t("Completato"),
        3: this.$t("Rilasciato"),
        4: this.$t("Sottomesso"),
        5: this.$t("Approvato"),
        // 6: "Imposta il documento come template",
        // 7: "Cataloga documento",
        8: this.$t("Checkout"),
        // 9: "Modifica Titolo",
        // 10: "Da convertire in PDF",
        // 11: "Convertito in PDF",
        12: this.$t("Sospeso"),
        20: this.$t("Collegamento")
      };
    },

    detail_options() {
      return [
        {
          id: 1000,
          text: this.$t("DaGestire"),
          icon: "fas fa-ban",
          iconColor: "red",
          action: "d_setDocumentToBeCreated",
          detail_states: [2, 12, 20]
        },
        {
          id: 1001,
          text: this.$t("InGestione"),
          icon: "fas fa-edit",
          iconColor: "#01579B",
          action: "d_setDocumentToUnderProcess",
          detail_states: [2, 12]
        },
        {
          id: 1002,
          text: this.$t("Completato"),
          icon: "fas fa-check",
          iconColor: "green",
          action: "d_setDocumentToCompleted",
          detail_states: [1]
        },
        {
          id: 1003,
          text: this.$t("Rilasciato"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToReleased",
          detail_states: []
        },
        {
          id: 1004,
          text: this.$t("Sottomesso"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToSubmitted",
          detail_states: []
        },
        {
          id: 1005,
          text: this.$t("Approvato"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToApproved",
          detail_states: []
        },
        {
          id: 1006,
          text: this.$t("ImpostaDocComeTemplate"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToTemplate",
          detail_states: []
        },
        {
          id: 1007,
          text: this.$t("CatalogaDocumento"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToCataloged",
          detail_states: []
        },
        {
          id: 1008,
          text: this.$t("Checkout"),
          icon: "fas fa-arrow-circle-down",
          iconColor: "#EF6C00",
          action: "d_setDocumentToCheckOut",
          detail_states: [1]
        },
        {
          id: 1009,
          text: this.$t("ModificaTitolo"),
          icon: "fas fa-i-cursor",
          iconColor: "black",
          action: "d_setDocumentToRename",
          detail_states: [1, 2]
        },
        {
          id: 1010,
          text: this.$t("Sospeso"),
          icon: "fas fa-pause",
          iconColor: "#FFCA28",
          action: "d_setDocumentToStandBy",
          detail_states: [1]
        },
        {
          id: 1011,
          text: this.$t("ContinuazioneAllegato"),
          icon: "fas fa-external-link-alt",
          iconColor: "black",
          action: "d_setNewDocumentAttach",
          detail_states: [1, 2]
        },
        {
          id: 1012,
          text: this.$t("AnnullaDocumento"),
          icon: "fas fa-minus-square",
          iconColor: "red",
          action: "d_setDocumentToCancel",
          detail_states: [1]
        },
        {
          id: 1013,
          text: this.$t("RientraDaCheckout"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToRestore",
          detail_states: [8]
        },
        {
          id: 1014,
          text: this.$t("Proprieta"),
          icon: "fas fa-info-circle",
          iconColor: "blue",
          action: "d_getInfo",
          detail_states: [1, 2, 8, 12]
        },
        {
          id: 1015,
          text: this.$t("ReperimentoDocumento"),
          icon: "fas fa-history",
          iconColor: "red",
          action: "d_ReperimentoDocumento",
          detail_states: [1]
        },
        {
          id: 1016,
          text: this.$t("CreaCartella"),
          icon: "fas fa-folder-plus",
          iconColor: "#FFD54F",
          action: "d_newFolder",
          detail_states: [0]
        },
        {
          id: 1017,
          text: this.$t("RinominaCartella"),
          icon: "fas fa-i-cursor",
          iconColor: "black",
          action: "d_renameFolder",
          detail_states: [0]
        },
        {
          id: 1018,
          text: this.$t("CancellaCartella"),
          icon: "fas fa-folder-minus",
          iconColor: "red",
          action: "d_deleteFolder",
          detail_states: [0]
        },
        {
          id: 1019,
          text: this.$t("CreaFile"),
          icon: "fas fa-plus-square",
          iconColor: "#01579B",
          action: "d_setNewDocument",
          detail_states: [0]
        },
        {
          id: 1020,
          text: this.$t("CollegamentoFile"),
          icon: "fas fa-link",
          iconColor: "#01579B",
          action: "d_setExternalPath",
          detail_states: [1]
        },
        {
          id: 1021,
          text: this.$t("ModificaCollegamentoFile"),
          icon: "fas fa-link",
          iconColor: "success",
          action: "d_setExternalPath",
          detail_states: [20]
        }
      ];
    }

    // companyRole: {
    //   get() {
    //     var r = {
    //       companyRoleID:
    //         this.company.companyInfo.companyRoleID == undefined
    //           ? 0
    //           : this.company.companyInfo.companyRoleID,
    //       companyRoleName:
    //         this.company.companyInfo.companyRoleName == undefined
    //           ? ""
    //           : this.company.companyInfo.companyRoleName
    //     };
    //     return r;
    //   },
    //   set(v) {
    //     console.log("computed\\companyRole\\set");

    //     if (typeof v === "object" && v !== null) {
    //       this.company.companyInfo.companyRoleID = v.companyRoleID;
    //       this.company.companyInfo.companyRoleName = v.companyRoleName;
    //     } else {
    //       this.company.companyInfo.companyRoleID = 0;
    //       this.company.companyInfo.companyRoleName = v;
    //     }

    //     var r = {
    //       companyRoleID:
    //         this.company.companyInfo.companyRoleID == undefined
    //           ? 0
    //           : this.company.companyInfo.companyRoleID,
    //       companyRoleName:
    //         this.company.companyInfo.companyRoleName == undefined
    //           ? ""
    //           : this.company.companyInfo.companyRoleName
    //     };
    //     return r;
    //   }
    // },
    // computedClass: {
    //   get() {
    //     var r = {
    //       mdClassID:
    //         this.task.taskInfo.mdClassID == undefined
    //           ? 0
    //           : this.task.taskInfo.mdClassID,
    //       mdClassName:
    //         this.task.taskInfo.mdClassName == undefined
    //           ? ""
    //           : this.task.taskInfo.mdClassName
    //     };
    //     return r;
    //   },
    //   set(v) {
    //     console.log("computed\\companyRole\\set");

    //     if (typeof v === "object" && v !== null) {
    //       this.task.taskInfo.mdClassID = v.mdClassID;
    //       this.task.taskInfo.mdClassName = v.mdClassName;
    //     } else {
    //       this.task.taskInfo.mdClassID = 0;
    //       this.task.taskInfo.mdClassName = v;
    //     }

    //     var r = {
    //       mdClassID:
    //         this.task.taskInfo.mdClassID == undefined
    //           ? 0
    //           : this.task.taskInfo.mdClassID,
    //       mdClassName:
    //         this.task.taskInfo.mdClassName == undefined
    //           ? ""
    //           : this.task.taskInfo.mdClassName
    //     };
    //     return r;
    //   }
    // },
    // computedProduct: {
    //   get() {
    //     var r = {
    //       productID:
    //         this.task.taskInfo.productID == undefined
    //           ? 0
    //           : this.task.taskInfo.productID,
    //       productName:
    //         this.task.taskInfo.productName == undefined
    //           ? ""
    //           : this.task.taskInfo.productName
    //     };
    //     return r;
    //   },
    //   set(v) {
    //     console.log("computed\\companyRole\\set");

    //     if (typeof v === "object" && v !== null) {
    //       this.task.taskInfo.productID = v.productID;
    //       this.task.taskInfo.productName = v.productName;
    //     } else {
    //       this.task.taskInfo.productID = 0;
    //       this.task.taskInfo.productName = v;
    //     }

    //     var r = {
    //       productID:
    //         this.task.taskInfo.productID == undefined
    //           ? 0
    //           : this.task.taskInfo.productID,
    //       productName:
    //         this.task.taskInfo.productName == undefined
    //           ? ""
    //           : this.task.taskInfo.productName
    //     };
    //     return r;
    //   }
    // },
    // computedmdActivity: {
    //   get() {
    //     var r = {
    //       mdActivityID:
    //         this.task.taskInfo.mdActivityID == undefined
    //           ? 0
    //           : this.task.taskInfo.mdActivityID,
    //       mdActivityName:
    //         this.task.taskInfo.mdActivityName == undefined
    //           ? ""
    //           : this.task.taskInfo.mdActivityName
    //     };
    //     return r;
    //   },
    //   set(v) {
    //     console.log("computed\\companyRole\\set");

    //     if (typeof v === "object" && v !== null) {
    //       this.task.taskInfo.mdActivityID = v.mdActivityID;
    //       this.task.taskInfo.mdActivityName = v.mdActivityName;
    //     } else {
    //       this.task.taskInfo.mdActivityID = 0;
    //       this.task.taskInfo.mdActivityName = v;
    //     }

    //     var r = {
    //       mdActivityID:
    //         this.task.taskInfo.mdActivityID == undefined
    //           ? 0
    //           : this.task.taskInfo.mdActivityID,
    //       mdActivityName:
    //         this.task.taskInfo.mdActivityName == undefined
    //           ? ""
    //           : this.task.taskInfo.mdActivityName
    //     };
    //     return r;
    //   }
    // },
    // computedEdition: {
    //   get() {
    //     return this.task.taskInfo.editionName == undefined
    //       ? ""
    //       : this.task.taskInfo.editionName;
    //   },
    //   set(v) {
    //     this.task.taskInfo.editionID = 0;
    //     this.task.taskInfo.editionName = v;

    //     return this.task.taskInfo.editionName == undefined
    //       ? ""
    //       : this.task.taskInfo.editionName;
    //   }
    // },
    // computedStructure: {
    //   get() {
    //     var r = {
    //       structureID:
    //         this.task.taskInfo.structureID == undefined
    //           ? 0
    //           : this.task.taskInfo.structureID,
    //       structureName:
    //         this.task.taskInfo.structureName == undefined
    //           ? ""
    //           : this.task.taskInfo.structureName
    //     };
    //     return r;
    //   },
    //   set(v) {
    //     console.log("computed\\companyRole\\set");

    //     if (typeof v === "object" && v !== null) {
    //       this.task.taskInfo.structureID = v.structureID;
    //       this.task.taskInfo.structureName = v.structureName;
    //     } else {
    //       this.task.taskInfo.structureID = 0;
    //       this.task.taskInfo.structureName = v;
    //     }

    //     var r = {
    //       structureID:
    //         this.task.taskInfo.structureID == undefined
    //           ? 0
    //           : this.task.taskInfo.structureID,
    //       structureName:
    //         this.task.taskInfo.structureName == undefined
    //           ? ""
    //           : this.task.taskInfo.structureName
    //     };
    //     return r;
    //   }
    // }
  }, // contiene un oggetto che definisce le funzioni getter e setter per le proprietà calcolate del componente Vue. Le proprietà calcolate influenzano un aggiornamento reattivo sul DOM ogni volta che il loro valore cambia.
  props: {}, //contiene un array o un oggetto di proprietà specifiche del componente Vue.js, impostato al momento dell'invocazione.
  watch: {
    editionID(v) {
      this.loadDataList();
    }
  }, // questo oggetto tiene traccia dei cambiamenti nel valore di una qualsiasi delle proprietà definite come parte dei "dati" impostando le funzioni per controllarli.
  //Eventi------------------------
  beforeCreate() {}, //questo è il primo stato del ciclo di vita. Non puoi ancora interagire con nessuna parte del componente
  created() {}, //questo è subito dopo la creazione dell'istanza del componente. Ora puoi interagire con il componente, ad es. le proprietà dei dati, i watcher, le proprietà calcolate, ma non puoi ancora accedere al DOM. Di solito, i dati vengono recuperati dal database o dall'API in questo hook del ciclo di vita.
  beforeMount() {}, //il componente è compilato in questa fase, ma deve ancora essere visualizzato sullo schermo.
  mounted() {
    this.loadDataList();
  }, //questo avviene dopo che il componente è stato montato. Ora puoi accedere al metodo $ el e giocare con il contenuto all'interno degli elementi HTML. In questa fase il componente diventa completamente interattivo.
  beforeUpdate() {}, //ogni volta che vengono apportate modifiche ai dati o al DOM, subito prima, viene chiamato questo hook del ciclo di vita. Ciò è utile quando è necessario registrare le modifiche.
  updated() {}, //subito dopo che sono state apportate le modifiche al DOM o ai dati. Qui puoi eseguire operazioni dipendenti dalla modifica nel DOM.
  beforeDestroy() {}, //questo è subito prima che il componente venga distrutto ed è l'ultima istanza del DOM completamente funzionante. Puoi eseguire le operazioni di chiusura necessarie.
  destroyed() {} //questo è un po 'simile all'hook beforeCreate in cui il componente non è funzionale e non è possibile accedere a proprietà dei dati, watcher, proprietà calcolate ed eventi.
};
</script>

<style scoped>
.v-input:not(.v-textarea) {
  max-height: 50px;
}
</style>
