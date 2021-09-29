<template>
  <v-row class="px-2">
    <v-overlay color="primary" opacity="0.7" v-if="uploadPercentage > 0">
      <h1>
        <v-icon x-large class="mr-3">fas fa-cloud-upload-alt</v-icon
        >{{ uploadPercentage }}
        <v-icon>fas fa-percentage</v-icon>
      </h1>
    </v-overlay>

    <v-col class="text-center" cols="12">
      <blockquote class="blockquote">
        &#8220;ACCESSO FABBRICANTE&#8221;
        <footer>
          <small>
            <em>&mdash; {{ userInfo.displayName }} ({{ $t("welcome") }})</em>
          </small>
        </footer>
      </blockquote>
    </v-col>
    <v-col cols="12">
      <v-card height="100%">
        <v-app-bar v-if="viewTree">
          <v-btn text @click="back">
            <v-icon class="mr-2">
              far fa-arrow-alt-circle-left
            </v-icon>
            Back
          </v-btn>
          <v-spacer />
          <v-icon small color="red" @click="clearArchive()">
            fas fa-trash-alt
          </v-icon>
        </v-app-bar>

        <v-card-title v-if="!viewTree">
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
          v-if="!viewTree"
        >
          <template v-slot:top>
            <v-toolbar flat>
              <v-toolbar-title
                ><v-icon color="primary" class="mr-2" @click="loadDataList()"
                  >fas fa-tasks</v-icon
                >
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
            <span>{{ item.productName }}</span>
          </template>

          <template v-slot:item.actions="{ item }">
            <v-progress-linear
              color="deep-purple accent-4"
              indeterminate
              rounded
              height="6"
              v-if="processingDocument(item.editionID)"
            ></v-progress-linear>

            <v-icon  class="mr-2" color="primary">
              fas fa-info-circle
            </v-icon>

            <v-file-input
              v-model="uploadFile"
              :rules="uploadRules"
              :accept="uploadAcceptType"
              hide-input
              :color="item.fileStatus > -1 ? 'primary' : 'error'"
              prepend-icon="fas fa-cloud-upload-alt"
              @click="resetAttachFile('upload' + item.editionID)"
              @change="attachFile('upload' + item.editionID, item)"
              :id="'upload' + item.editionID"
              :name="'upload' + item.editionID"
              :ref="'upload' + item.editionID"
              v-if="item.asZipFile && item.fileStatus < 1"
              dense
              :class="item.fileStatus == 0 ? 'mr-2' : 'mr-2 p-0 icon-error'"
            >
            </v-file-input>
            <v-btn
              icon
              x-small
              v-else-if="item.asZipFile && item.fileStatus > 0"
              @click="handleClick(item)"
              class="mr-2"
            >
              <v-icon color="success">
                fas fa-cloud-upload-alt
              </v-icon>
            </v-btn>

            <v-btn
              icon
              x-small
              v-if="!item.asZipFile"
              @click="handleClick(item)"
              class="mr-2"
            >
              <v-icon color="#ffc107">
                fas fa-sitemap
              </v-icon>
            </v-btn>


            <v-icon class="mr-2" color="primary">
              fas fa-tasks
            </v-icon>

            
          </template>
          <template v-slot:no-data>
            <v-btn color="primary" @click="loadDataList()">
              Refresh
            </v-btn>
          </template>
        </v-data-table>

        <tf-treeview v-if="viewTree" :editionID.sync="editionID" />
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import TfTreeview from "~/components/TfTreeview.vue";
export default {
  components: { TfTreeview },
  layout: "fab",
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
      uploadFiles: [],
      uploadFile: null,
      //  "application/zip, application/pdf, application/x-7z-compressed, application/vnd.rar",
      // uploadAcceptType:
      //   "application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/pdf, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      uploadRules: [
        value =>
          !value ||
          value.size < 2000000 ||
          "Il file dev'essere inferiore a 2 MB!"
      ],
      selectedDocument: null,
      selectedCompany: 0,
      inProgress: [],
      uploadPercentage: 0,
      viewTree: false,
      editionID: 0
    };
  },
  computed: {},

  watch: {
    uploading(val) {
      if (!val) return;
      //setTimeout(() => (this.uploading = false), 2000);
      //setTimeout(() => this.endProcess(this.selectedDocument.id), 2000);
    },
    uploadFiles(val) {
      if (!val) return;
      console.log("detailsList\\watch\\uploadFiles..");
      console.log("selectedDocument:");
      console.log(this.selectedDocument);
      var ext = val.name.split(".").pop();
      console.log("ext: " + ext);
      if (["zip", "7z", "gzip", "rar"].indexOf(ext) > -1) {
        this.startProcess(this.selectedDocument.editionID);
        this.uploading = true;
        this.uploadDocument();
        // } else if (["doc", "docx", "pdf", "xls", "xlsx"].indexOf(ext) > -1) {
        //   // this.setTemplate = true;
        //   // this.startProcess(this.selectedDocument.mdTaskID);
        //   // this.uploading = false;
      } else {
        this.viewMessage("error", "Formato non supportato", "Upload");
        this.uploading = false;
      }

      console.log(val);
    },
    uploadFile(val) {
      if (!val) return;
      console.log("detailsList\\watch\\uploadFile..");
      console.log("selectedDocument:");
      console.log(this.selectedDocument);
      var ext = val.name.split(".").pop();
      console.log("ext: " + ext);
      if (["zip", "7z", "rar", "gzip"].indexOf(ext) > -1) {
        this.startProcess(this.selectedDocument.editionID);
        this.uploading = true;
        this.uploadDocument();
        // } else if (["doc", "docx", "pdf", "xls", "xlsx"].indexOf(ext) > -1) {
        //   // this.setTemplate = true;
        //   // this.startProcess(this.selectedDocument.mdTaskID);
        //   // this.uploading = false;
      } else {
        this.viewMessage("error", "Formato non supportato", "Upload");
        this.uploading = false;
      }

      console.log(val);
    }
  },
  mounted() {
    this.loadDataList();
  },
  methods: {
    async loadDataList() {
      console.log("loadDataList..");
      this.loadData = true;
      var data = (
        await this.$axios.get("fabEditData/" + this.userInfo.companyID)
      ).data;
      this.Items = data.company.details.tasks;
      this.loadData = false;
    },
    attachFile(id, values) {
      this.uploading = true;
      console.log("dashboard\\methods\\attachFile:");
      console.log(id);
      // console.log(this.$("#" + id).val());
      //console.log(this.$("#upload11402").files[0]);
      console.log(values);
      this.selectedDocument = values;
    },
    resetAttachFile(id) {
      console.log("dashboard\\method\\resetAttachFile:");
      console.log(id);
      try {
        // this.$("#" + id).val("");
      } catch (error) {}
    },
    findItem(id, list) {
      console.log("fab\\dashboard\\methods\\finditems");
      list = list == undefined ? this.Items : list;
      for (var key in list) {
        if (list[key].editionID === id) {
          return list[key]; // return the object and stop further searching
        } else if (list[key].children && list[key].children.length) {
          // if the property is another object
          var res = this.findItem(id, list[key].children); // get the result of the search in that sub object
          if (res) return res; // return the result if the search was successful, otherwise don't return and move on to the next property
        }
      }
      return null; // return null or any default value you want if the search is unsuccessful (must be falsy to work)
    },
    updateItemData(d) {
      // this.findItem(d.id).name = d.name;
      // this.findItem(d.id).file = d.file;
      // this.findItem(d.id).owner = d.owner;
      // this.findItem(d.id).flag_contenitore = d.flag_contenitore;
      // this.findItem(d.id).flag_stato = d.flag_stato;
      // this.findItem(d.id).detail_id = d.detail_id;
      // this.findItem(d.id).id = d.id;
      // this.findItem(d.id).AddFolder = d.AddFolder;
      // this.findItem(d.id).AddFile = d.AddFile;
      // this.findItem(d.id).NLivelli = d.NLivelli;
      // this.findItem(d.id).IDparent = d.IDparent;
      // this.findItem(d.id).href = d.href;
      // this.findItem(d.id).IDutente = d.IDutente;
      this.$nextTick(() => {
        console.warn("Aggiornamento [nextTick] elemento: " + d.editionName);
        this.$set(
          this.findItem(d.editionID),
          "mdTaskStatesID",
          d.mdTaskStatesID
        );
        this.$set(this.findItem(d.editionID), "fileStatus", d.fileStatus);
        this.$set(this.findItem(d.editionID), "ownerID", d.ownerID);
        this.$set(this.findItem(d.editionID), "modifiedDate", d.modifiedDate);
        this.$set(
          this.findItem(d.editionID),
          "mdTaskStatesName",
          d.mdTaskStatesName
        );
        this.$set(this.findItem(d.editionID), "userName", d.userName);
        this.$set(this.findItem(d.editionID), "displayName", d.displayName);
        // this.$set(this.findItem(d.editionID), "AddFolder", d.AddFolder);
        // this.$set(this.findItem(d.editionID), "AddFile", d.AddFile);
        // this.$set(this.findItem(d.editionID), "NLivelli", d.NLivelli);
        // this.$set(this.findItem(d.editionID), "IDparent", d.IDparent);
        // this.$set(this.findItem(d.editionID), "href", d.href);
        // this.$set(this.findItem(d.editionID), "IDutente", d.IDutente);
        // this.$set(this.findItem(d.editionID), "ver", d.ver);
        // this.$set(this.findItem(d.editionID), "link", d.link);
        // this.$set(this.findItem(d.editionID), "Ext_id", d.Ext_id);
      });

      if (this.searchAdv) {
        this.filter(this.searchAdv);
      }
    },

    async uploadDocument() {
      //Carica il file sul server
      console.log("fab\\dashboard\\methods\\uploadDocument");
      this.uploading = true;

      var formData = new FormData();
      formData.append("editionID", this.selectedDocument.editionID);
      formData.append("userID", this.userInfo.userID);
      formData.append("file", this.uploadFile);
      this.$axios
        .post("actions/upload/archive", formData, {
          // headers: {
          //   "Content-Type": "multipart/form-data;"
          // },
          onUploadProgress: function(progressEvent) {
            this.uploadPercentage = parseInt(
              Math.round((progressEvent.loaded / progressEvent.total) * 100)
            );
            if (this.uploadPercentage == 100) {
              setTimeout(() => (this.uploadPercentage = 0), 3000);
            }
          }.bind(this)
        })
        .then(response => {
          console.log(response.data);
          var data = response.data;
          if (data.stato == -1) {
            this.viewMessage("error", data.messaggio, "Upload");
          } else {
            if (data.task) {
              this.updateItemData(data.task);
            } else {
              this.loadDataList();
            }
          }
          this.endProcess(this.selectedDocument.editionID);
          this.uploading = false;
          //this.getDetailID(this.selectedDocument.mdTaskID);
        })
        .catch(e => {
          this.endProcess(this.selectedDocument.editionID);
          this.uploading = false;
          this.viewMessageError(e, "Upload");
        });
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

    handleClick(row) {
      this.viewMessage("success", row.productName, row.BusinessName);
      this.editionID = row.editionID;
      this.viewTree = true;
    },
    back() {
      this.viewTree = false;
      this.loadDataList();
    },
    async clearArchive() {
      console.log("clearArchive..");
      this.loadData = true;
      var data = (await this.$axios.get("Actions/task/clearArchive/" + this.editionID))
        .data;
      if (data.stato == -1) {
        this.viewMessage("error", data.messaggio, "Clear archive");
      } else {
        if (data.task) {
          this.updateItemData(data.task);
        } else {
          this.loadDataList();
        }
        this.back();
      }
      this.loadData = false;
    }
  }
};
</script>

<style scoped>
.v-file-input:not(.icon-error) ::before {
  color: #1976d2;
  font-size: 1rem;
}
.v-file-input.icon-error ::before {
  color: #d21919;
  font-size: 1rem;
}
.v-file-input {
  width: 20px;
  height: 20px;
  display: inline;
}
</style>
