<template>
  <v-row>
    <v-overlay color="primary" opacity="0.7" v-if="uploadPercentage > 0">
      <h1>{{uploadPercentage}}</h1>
    </v-overlay>

    <v-col class="text-center">
      <img src="/tfo-logo.png" alt="Vuetify.js" class="mb-5" />
      <blockquote class="blockquote">
        &#8220;ACCESSO FABBRICANTE&#8221;
        <footer>
          <small>
            <em>&mdash; {{ userInfo.DisplayName }} ({{ $t("welcome") }})</em>
          </small>
        </footer>
      </blockquote>
    </v-col>
    <v-col class="text-center">
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
            <span>{{ item.productName }}</span>
          </template>

          <template v-slot:item.actions="{ item }">
            <v-icon small class="mr-2" color="primary" @click="usersItem(item)">
              fas fa-users-cog
            </v-icon>

            <v-file-input
              v-model="uploadFiles"
              :rules="uploadRules"
              :accept="uploadAcceptType"
              hide-input
              color="primary"
              :prepend-icon="
                item.StructureID == 1
                  ? 'fas fa-cloud-upload-alt'
                  : 'fas fa-sitemap'
              "
              @click="resetAttachFile('upload' + item.mdTaskID)"
              @change="attachFile('upload' + item.mdTaskID, item)"
              :id="'upload' + item.mdTaskID"
              :ref="'upload' + item.mdTaskID"
              dense
            >
            </v-file-input>

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
    </v-col>
  </v-row>
</template>

<script>
export default {
  components: {},
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
      uploadFiles: [],
      uploadAcceptType:
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/pdf, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
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
      if (["zip"].indexOf(ext) > -1) {
        this.startProcess(this.selectedDocument.mdTaskID);
        this.uploading = true;
        this.uploadDocument();
      } else if (["doc", "docx", "pdf", "xls", "xlsx"].indexOf(ext) > -1) {
        // this.setTemplate = true;
        // this.startProcess(this.selectedDocument.mdTaskID);
        // this.uploading = false;
        this.viewMessage("error", "Formato non supportato", "Upload");
      } else {
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
    uploadDocument() {
      //Carica il file sul server
      console.log("detailsList\\methods\\uploadTemplate");
      this.uploading = true;
      this.setTemplate = false;
      var formData = new FormData();

      formData.append("mdTaskID", this.selectedDocument.mdTaskID);
      formData.append("userID", this.userInfo.userID);
      this.$axios
        .post("actions/upload", formData, {
          headers: {
            "Content-Type": "multipart/form-data;"
          },
          onUploadProgress: function(progressEvent) {
            this.uploadPercentage = parseInt(
              Math.round((progressEvent.loaded / progressEvent.total) * 100)
            );
            if (this.uploadPercentage == 100){
              setTimeout(() => this.uploadPercentage = 0, 3000);
            }
          }.bind(this)
        })
        .then(response => {
          console.log(response.data);
          this.endProcess(this.selectedDocument.mdTaskID);
          this.uploading = false;
          //this.loadDataList();
          this.getDetailID(this.selectedDocument.mdTaskID);
        })
        .catch(e => {
          this.endProcess(this.selectedDocument.mdTaskID);
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
    }
  }
};
</script>

<style scoped>
.v-file-input ::before {
  color: #1976d2;
  font-size: 1rem;
}
.v-file-input {
  width: 20px;
  height: 20px;
  display: inline;
}
</style>
