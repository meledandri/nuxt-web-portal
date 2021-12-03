<template>
  <!-- Gestione STATO -->
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
        <v-icon right x-small>fas fa-chevron-down</v-icon>
      </v-btn>
    </template>
    <v-list dense>
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
      2: "success"
    },
    ApprovalWorkflowEnabled: false,
    archive: 1
  }),
  computed: {
    userCanChange: {
      get() {
        return true;
      }
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
      console.log("folderOptions\limitOptios..");
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
            var AddFolder = item.addFolder;
            var AddFile = item.addFile;
            var NLivelli = item.nLevels;
            var idVerDoc = item.ver;
            //console.log("options id..");
            //console.log(options[opt].id);
            //console.log(options[opt].text);

            switch (
              options[opt].id //In base all'azione richiesta possiamo filtrare maggiormente in base ai dati del documento
            ) {
              case 1009: //Modifica titolo
                check = item.documentID > 1000000000000000; // Solo se idDocumento > 1000000000000000
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
                check = item.documentID < 1000000000000000; // Solo se idDocumento < 1000000000000000
                if (check) {
                  o.push(options[opt]);
                }
                break;
              case 1012: //Annulla Documento
                check = item.documentID > 1000000000000000; // Solo se idDocumento > 1000000000000000
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
                  // Se ha l'id documento breve
                  o.push(options[opt]);
                }
                break;
              case 1018: //Cancella cartella
                if (fc == 2) {
                  // Se si tratta di una cartella (flag_contentore = 2)
                  // Se ha l'id documento breve
                  check = item.userOwner;
                  if (check) {
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
                console.log("folderOptions\\methods\\limitOptions (default)");
                console.log("#############################################");
              // console.log("options id..")
              // console.log(options[opt].id)
            }
          } else {
            console.log("state:" + state);
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
    }
  }
};
</script>

<style scoped>
.v-btn.btn-tab {
  width: 136px;
}
</style>
