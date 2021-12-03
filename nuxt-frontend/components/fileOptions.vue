<template>
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
        :color="stateColor(item)"
        :dark="item.flagState != 0"
        :outlined="item.flagState == 0"
        v-bind="attrs"
        v-on="on"
        :disabled="item.flagState == 0"
        class="btn-tab"
      >
        {{ getStateName(item.flagState) }}
        <v-icon
          right
          x-small
          v-if="item.flagState != 0"
          >fas fa-chevron-down</v-icon
        >
      </v-btn>
    </template>

    <v-list
      dense
    >
      <v-list-item
        v-for="(citem, index) in limitOptions(item)"
        :key="index"
        @click="fn_setAction(citem.action, item)"
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
</template>
<script>
export default {
  name: "FileOptions",
  props: ["item", "product", "links_states"],
  data: () => ({
        link_states: {
      "-1": "error",
      0: "grey",
      1: "warning",
      2: "success",
    },
    ApprovalWorkflowEnabled: false,
    archive: 1,

  }),
  computed: {
    userCanChange: {
      get() {
      return  true;
      },
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
        20: this.$t("Collegamento"),
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
          detail_states: [2, 12, 20],
        },
        {
          id: 1001,
          text: this.$t("InGestione"),
          icon: "fas fa-edit",
          iconColor: "#01579B",
          action: "d_setDocumentToUnderProcess",
          detail_states: [ 12],
        },
        {
          id: 1002,
          text: this.$t("Completato"),
          icon: "fas fa-check",
          iconColor: "green",
          action: "d_setDocumentToCompleted",
          detail_states: [1],
        },
        {
          id: 1003,
          text: this.$t("Rilasciato"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToReleased",
          detail_states: [],
        },
        {
          id: 1004,
          text: this.$t("Sottomesso"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToSubmitted",
          detail_states: [],
        },
        {
          id: 1005,
          text: this.$t("Approvato"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToApproved",
          detail_states: [],
        },
        {
          id: 1006,
          text: this.$t("ImpostaDocComeTemplate"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToTemplate",
          detail_states: [],
        },
        {
          id: 1007,
          text: this.$t("CatalogaDocumento"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToCataloged",
          detail_states: [],
        },
        {
          id: 1008,
          text: this.$t("Checkout"),
          icon: "fas fa-arrow-circle-down",
          iconColor: "#EF6C00",
          action: "d_setDocumentToCheckOut",
          detail_states: [1],
        },
        {
          id: 1009,
          text: this.$t("ModificaTitolo"),
          icon: "fas fa-i-cursor",
          iconColor: "black",
          action: "d_setDocumentToRename",
          detail_states: [1, 2],
        },
        {
          id: 1010,
          text: this.$t("Sospeso"),
          icon: "fas fa-pause",
          iconColor: "#FFCA28",
          action: "d_setDocumentToStandBy",
          detail_states: [1],
        },
        {
          id: 1011,
          text: this.$t("ContinuazioneAllegato"),
          icon: "fas fa-external-link-alt",
          iconColor: "black",
          action: "d_setNewDocumentAttach",
          detail_states: [1, 2],
        },
        {
          id: 1012,
          text: this.$t("AnnullaDocumento"),
          icon: "fas fa-minus-square",
          iconColor: "red",
          action: "d_setDocumentToCancel",
          detail_states: [2],
        },
        {
          id: 1013,
          text: this.$t("RientraDaCheckout"),
          icon: "fas fa-circle",
          iconColor: "red",
          action: "d_setDocumentToRestore",
          detail_states: [8],
        },
        {
          id: 1014,
          text: this.$t("Proprieta"),
          icon: "fas fa-info-circle",
          iconColor: "blue",
          action: "d_getInfo",
          detail_states: [1, 2, 8, 12],
        },
        {
          id: 1015,
          text: this.$t("ReperimentoDocumento"),
          icon: "fas fa-history",
          iconColor: "red",
          action: "d_ReperimentoDocumento",
          detail_states: [1],
        },
        {
          id: 1016,
          text: this.$t("CreaCartella"),
          icon: "fas fa-folder-plus",
          iconColor: "#FFD54F",
          action: "d_newFolder",
          detail_states: [0],
        },
        {
          id: 1017,
          text: this.$t("RinominaCartella"),
          icon: "fas fa-i-cursor",
          iconColor: "black",
          action: "d_renameFolder",
          detail_states: [0],
        },
        {
          id: 1018,
          text: this.$t("CancellaCartella"),
          icon: "fas fa-folder-minus",
          iconColor: "red",
          action: "d_deleteFolder",
          detail_states: [0],
        },
        {
          id: 1019,
          text: this.$t("CreaFile"),
          icon: "fas fa-plus-square",
          iconColor: "#01579B",
          action: "d_setNewDocument",
          detail_states: [0],
        },
        {
          id: 1020,
          text: this.$t("CollegamentoFile"),
          icon: "fas fa-link",
          iconColor: "#01579B",
          action: "d_setExternalPath",
          detail_states: [1],
        },
        {
          id: 1021,
          text: this.$t("ModificaCollegamentoFile"),
          icon: "fas fa-link",
          iconColor: "success",
          action: "d_setExternalPath",
          detail_states: [20],
        },
      ];
    },
  },
  methods: {
    fn_changeAuthor(item, user) {
      this.$emit("changeAuthor", item, user);
    },
    fn_setAction(action, item) {
      this.$emit("setAction", action, item);
    },
    getStateName(val) {
      //Recupera il nome dello stato in base al codice
      return this.detail_states[val];
    },
    stateColor(row) {
      var s = row.flagState;
      var c =
        s == 2
          ? "#43A047"
          : s == 12
          ? "#FBC02D"
          : s == 8
          ? "#E65100"
          : s == 20 && row.link != null
          ? this.link_states[
              this.findItem(this.item.id, this.links_states).link_state
            ]
          : "#0D47A1";
      return c;
    },
    limitOptions(item) {
      // Crea un elenco delle voci contestuali in base allo stato
      var options = this.detail_options;
      console.log("fileOptions\limitOptios..")
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
            var fc = item.flagContainer;
            var idd = item.documentID;
            var AddFolder = item.AddFolder;
            var AddFile = item.AddFile;
            var NLivelli = item.NLivelli;
            var idVerDoc = item.ver;
            //console.log("options id..");
            //console.log(options[opt].id);
            //console.log(options[opt].text);

            switch (
              options[opt].id //In base all'azione richiesta possiamo filtrare maggiormente in base ai dati del documento
            ) {
              case 1009: //Modifica titolo
                check = item.userOwner
                if (check) {
                  o.push(options[opt]);
                }
                break;
              case 1000: //Da gestire
                // if (item.flagState == 12 || item.flagState == 20) {
                //   // Solo se è un doc in sospeso
                //   o.push(options[opt]);
                // } else {
                  check = item.fileExtension == "pdf"; // Solo se file PDF
                  if (check) {
                    o.push(options[opt]);
                  }
                // }
                break;
              case 1011: //Continuazione Allegato
                check = !item.userOwner
                if (check) {
                  o.push(options[opt]);
                }
                break;
              case 1012: //Annulla Documento
              console.log("AnnullaDoc..")
                check = item.userOwner
                if (check) {
                  o.push(options[opt]);
                }
                break;
              case 1015: //Ripristino documento
                check = item.ver > 1; // Solo se idVerDoc > 1
                if (check) {
                  o.push(options[opt]);
                }
                break;
              case 1016: //Crea nuova cartella
                if (fc == 2) {
                  // Se si tratta di una cartella (flag_contentore = 2)
                  if (AddFolder > 0 && NLivelli > 0) {
                    o.push(options[opt]);
                  }
                }
                break;

              case 1017: //Rinomina cartella
                if (fc == 2) {
                  // Se si tratta di una cartella (flag_contentore = 2)
                  if (idd > 1000000000000000) {
                    // Se ha l'id documento breve
                    o.push(options[opt]);
                  }
                }
                break;
              case 1018: //Cancella cartella
                if (fc == 2) {
                  // Se si tratta di una cartella (flag_contentore = 2)
                  if (idd > 1000000000000000) {
                    // Se ha l'id documento breve
                    o.push(options[opt]);
                  }
                }
                break;
              case 1019: //Crea nuovo file
                if (fc == 2) {
                  // Se si tratta di una cartella (flag_contentore = 2)
                  if (AddFile > 0) {
                    o.push(options[opt]);
                  }
                }
                break;

              case 1020: //Collega file solo se idVerDoc è 0
                if (fc == 1) {
                  // Se si tratta di una cartella (flag_contentore = 2)
                  if (idVerDoc == 0) {
                    o.push(options[opt]);
                  }
                }
                break;

              default:
                o.push(options[opt]);
                console.log("fileOptions\\methods\\limitOptions (default)");
                console.log('ooooo')
              // console.log("options id..")
              // console.log(options[opt].id)
            }
          } else {
            console.log("state:" + state)
          }
        }
        return o;
      }
      console.log(item);
      return null;
    },
        findItem(id, list) {
      console.log("fileOptions\\methods\\finditems");
      list = list == undefined ? this.items : list;
      for (var key in list) {
        if (list[key].id === id) {
          return list[key]; // return the object and stop further searching
        } else if (list[key].children && list[key].children.length) {
          // if the property is another object
          var res = this.findItem(id, list[key].children); // get the result of the search in that sub object
          if (res) return res; // return the result if the search was successful, otherwise don't return and move on to the next property
        }
      }
      return null; // return null or any default value you want if the search is unsuccessful (must be falsy to work)
    },

  },
};
</script>

<style scoped>
.v-btn.btn-tab {
  width: 136px;
}

</style>