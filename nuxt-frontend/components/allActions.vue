<template>
  <v-container fluid fill-height>
    <v-snackbar
      v-for="(msg, index) in errors"
      :key="index"
      v-model="popup.snackbar"
      :bottom="popup.y === 'bottom'"
      :color="popup.color"
      :left="popup.x === 'left'"
      :multi-line="popup.mode === 'multi-line'"
      :right="popup.x === 'right'"
      :timeout="popup.timeout"
      :top="popup.y === 'top'"
      :vertical="popup.mode === 'vertical'"
    >
      {{ msg }}

      <template v-slot:action="{ attrs }">
        <v-btn dark text v-bind="attrs" @click="popup.snackbar = false">
          Close
        </v-btn>
      </template>
    </v-snackbar>
    <v-layout justify-center align-center>
      <v-flex text-xs-center>
        <v-card class="card-h-100">
          <v-card-title class="px-0 py-0">
            <v-tabs v-model="tab" background-color="primary" dark show-arrows>
              <v-btn
                class=""
                @click="closeAction()"
                icon
                :disabled="postProduct"
                ><v-icon small color="white">fas fa-times</v-icon></v-btn
              >

              <!-- ------------------------------------------------------------------------------------------------TAB PRODOTTI -->

              <v-tab v-if="tabVisibility(0)">
                <!-- Nuovo Prodotto -->
                {{ $t("Nuovo") }} {{ $t("Prodotto") }}
              </v-tab>
              <v-tab v-if="tabVisibility(1)">
                <!-- Informazioni Prodotto -->
                {{ $t("InfoProdotto") }}
              </v-tab>
              <v-tab v-if="tabVisibility(2)">
                <!-- Modifica Prodotto -->
                {{ $t("Modifica") }} {{ $t("Prodotto") }}
              </v-tab>
              <v-tab v-if="tabVisibility(3)">
                <!-- Cancella Prodotto -->
                {{ $t("Cancella") }} {{ $t("Prodotto") }}
              </v-tab>
              <v-tab v-if="tabVisibility(4)">
                <!-- Qualifica Prodotto -->
                {{ $t("Qualifica") }} {{ $t("Prodotto") }}
              </v-tab>
              <v-tab v-if="tabVisibility(5)">
                <!-- Nuova Edizione Prodotto -->
                {{ $t("NuovaEdizione") }}
              </v-tab>
              <v-tab v-if="tabVisibility(6)">
                <!-- Ripristina in gestione -->
                {{ $t("RipristinaInGestione") }}
              </v-tab>
              <v-tab v-if="tabVisibility(7)">
                <!-- Approva Technical File -->
                {{ $t("ApprovaTechnicalFile") }}
              </v-tab>

              <!-- ------------------------------------------------------------------------------------------------TAB DETTAGLI -->

              <v-tab v-if="tabVisibility(10)">
                <!-- Nuovo File Documento ALLEGATO EX 0-->
                {{ $t("NuovoDocumento") }}
              </v-tab>
              <v-tab v-if="tabVisibility(11)">
                <!-- Informazioni Documento -->
                {{ $t("InfoProdotto") }}
              </v-tab>
              <v-tab v-if="tabVisibility(12)">
                <!-- Modifica Titolo -->
                {{ $t("ModificaTitolo") }}
              </v-tab>
              <v-tab v-if="tabVisibility(13)">
                <!-- Annulla Documento -->
                {{ $t("AnnullaDocumento") }}
              </v-tab>
              <v-tab v-if="tabVisibility(14)">
                <!-- Rinomina cartella -->
                {{ $t("RinominaCartella") }}
              </v-tab>
              <v-tab v-if="tabVisibility(15)">
                <!-- Nuovo Documento-->
                {{ $t("NuovoDocumento") }}
              </v-tab>
              <v-tab v-if="tabVisibility(16)">
                <!-- Cancella Cartella -->
                {{ $t("CancellaCartella") }}
              </v-tab>
              <v-tab v-if="tabVisibility(17)">
                <!-- Note documento -->
                {{ $t("NoteDocumento") }}
              </v-tab>
              <v-tab v-if="tabVisibility(18)">
                <!-- Nota -->
                {{ $t("Nota") }}
              </v-tab>

              <v-tab v-if="tabVisibility(19)">
                <!-- Genera revision list -->
                {{ $t("GeneraRevisionList") }}
              </v-tab>
              <v-tab v-if="tabVisibility(20)">
                <!-- Reperimento documento -->
                {{ $t("ReperimentoDocumento") }}
              </v-tab>
              <v-tab v-if="tabVisibility(21)">
                <!-- Upload multiplo -->
                {{ $t("CaricamentoMultiplo") }}
              </v-tab>
            </v-tabs>
          </v-card-title>
          <v-card-text class="inner-card">
            <v-tabs-items v-model="tab">
              <!-- ------------------------------------------------------------------------------------------------TAB PRODOTTI -->

              <!-- Nuovo technical File -->
              <v-tab-item v-if="tabVisibility(0)">
                <div style="color:red;" v-if="!temporary_file_present">
                  <h3>
                    {{ msg_temporary_file_not_present }}
                  </h3>
                </div>
                <div v-else>
                  <v-text-field
                    v-model="formData.description"
                    :label="$t('Descrizione')"
                  ></v-text-field>

                  <v-text-field
                    v-model="formData.version"
                    :label="$t('Edizione')"
                  ></v-text-field>

                  <v-combobox
                    v-model="formData.notifiedEntity"
                    :items="notifiedEntities"
                    item-text="label"
                    item-value="label"
                    :label="$t('OrganismoNotificato')"
                    :loading="loadingDataContext"
                    :return-object="false"
                  ></v-combobox>

                  <v-combobox
                    v-model="formData.MedicalDeviceName"
                    :items="medicalDevices"
                    item-text="label"
                    item-value="label"
                    :label="$t('MedicalDevice')"
                    :loading="loadingDataContext"
                    :return-object="false"
                  ></v-combobox>

                  <v-text-field
                    v-model="CompanyName"
                    :label="$t('Titolare')"
                    readonly
                    filled
                  ></v-text-field>

                  <v-select
                    v-model="formData.language"
                    item-text="label"
                    item-value="id"
                    :items="languages"
                    :label="$t('Lingua')"
                    dense
                  ></v-select>

                  <v-menu
                    v-model="formData.deadline_show"
                    :close-on-content-click="false"
                    transition="scale-transition"
                    offset-y
                    max-width="290px"
                    min-width="290px"
                  >
                    <template v-slot:activator="{ on, attrs }">
                      <v-text-field
                        v-model="computedDateFormatted"
                        :label="$t('DataScadenza')"
                        prepend-icon="fas fa-calendar-alt"
                        readonly
                        v-bind="attrs"
                        v-on="on"
                      ></v-text-field>
                    </template>
                    <v-date-picker
                      v-model="formData.deadline"
                      no-title
                      @input="formData.deadline_show = false"
                    ></v-date-picker>
                  </v-menu>

                  <v-btn
                    block
                    color="success"
                    @click="createProduct()"
                    dark
                    class="my-1"
                    v-if="temporary_file_present"
                    :loading="postProduct"
                    >{{ $t("CreaTechnicalFile") }}</v-btn
                  >
                </div>
              </v-tab-item>
              <!-- Proprietà -->
              <v-tab-item v-if="tabVisibility(1)">
                <v-text-field
                  v-model="product.descrizione"
                  :label="$t('Descrizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-font"
                ></v-text-field>

                <v-text-field
                  v-model="product.SiglaDossier"
                  :label="$t('Edizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-code-branch"
                ></v-text-field>

                <v-combobox
                  v-model="product.dosaggio"
                  :items="notifiedEntities"
                  item-text="label"
                  item-value="label"
                  :label="$t('OrganismoNotificato')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="far fa-dot-circle"
                ></v-combobox>

                <v-combobox
                  v-model="product.farmaco"
                  :items="medicalDevices"
                  item-text="label"
                  item-value="label"
                  :label="$t('MedicalDevice')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="fas fa-stethoscope"
                ></v-combobox>

                <v-text-field
                  v-model="product.officina"
                  :label="$t('Titolare')"
                  readonly
                  filled
                  prepend-icon="fas fa-building"
                ></v-text-field>

                <v-select
                  v-model="product.Language"
                  item-text="label"
                  item-value="id"
                  :items="languages"
                  :label="$t('Lingua')"
                  dense
                  readonly
                  filled
                  prepend-icon="fas fa-language"
                ></v-select>

                <v-text-field
                  v-model="computedDateFormattedInfo"
                  :label="$t('DataScadenza')"
                  readonly
                  filled
                  prepend-icon="fas fa-calendar-alt"
                ></v-text-field>
              </v-tab-item>

              <!-- Modifica Prodotto -->
              <v-tab-item v-if="tabVisibility(2)">
                <v-text-field
                  v-model="formData.description"
                  :label="$t('Descrizione')"
                  dense
                  prepend-icon="fas fa-font"
                ></v-text-field>

                <v-text-field
                  v-model="formData.version"
                  :label="$t('Edizione')"
                  dense
                  prepend-icon="fas fa-code-branch"
                ></v-text-field>

                <v-combobox
                  v-model="formData.notifiedEntity"
                  :items.sync="notifiedEntities"
                  item-text="label"
                  item-value="label"
                  :label="$t('OrganismoNotificato')"
                  loading="loadingDataContext"
                  :return-object="false"
                  prepend-icon="far fa-dot-circle"
                  dense
                ></v-combobox>

                <!-- <v-combobox
                  v-model="product.farmaco"
                  :items.sync="medicalDevices"
                  item-text="label"
                  item-value="label"
                  :label="$t('MedicalDevice')"
                  loading="loadingDataContext"
                  prepend-icon="fas fa-stethoscope"
                  dense
                ></v-combobox> -->

                <v-combobox
                  v-model="formData.MedicalDeviceName"
                  :items="medicalDevices"
                  item-text="label"
                  item-value="label"
                  :label="$t('MedicalDevice')"
                  :loading="loadingDataContext"
                  :return-object="false"
                ></v-combobox>

                <v-text-field
                  v-model="product.officina"
                  :label="$t('Titolare')"
                  readonly
                  filled
                  prepend-icon="fas fa-building"
                  dense
                ></v-text-field>

                <v-select
                  v-model="product.Language"
                  item-text="label"
                  item-value="id"
                  :items="languages"
                  :label="$t('Lingua')"
                  readonly
                  filled
                  prepend-icon="fas fa-language"
                  dense
                ></v-select>

                <v-menu
                  v-model="formData.deadline_show"
                  :close-on-content-click="false"
                  transition="scale-transition"
                  offset-y
                  max-width="290px"
                  min-width="290px"
                >
                  <template v-slot:activator="{ on, attrs }">
                    <v-text-field
                      v-model="computedDateFormatted"
                      :label="$t('DataScadenza')"
                      prepend-icon="fas fa-calendar-alt"
                      readonly
                      v-bind="attrs"
                      v-on="on"
                    ></v-text-field>
                  </template>
                  <v-date-picker
                    v-model="formData.deadline"
                    no-title
                    @input="formData.deadline_show = false"
                  ></v-date-picker>
                </v-menu>

                <v-btn
                  block
                  color="success"
                  @click="saveProduct()"
                  :loading="postProduct"
                  small
                  class="my-1"
                  >{{ $t("Salva") }}</v-btn
                >
              </v-tab-item>

              <!-- Cancella Prodotto-->
              <v-tab-item v-if="tabVisibility(3)">
                <v-text-field
                  v-model="product.descrizione"
                  :label="$t('Descrizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-font"
                ></v-text-field>

                <v-text-field
                  v-model="product.SiglaDossier"
                  :label="$t('Edizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-code-branch"
                ></v-text-field>

                <v-combobox
                  v-model="product.dosaggio"
                  :items="notifiedEntities"
                  item-text="label"
                  item-value="label"
                  :label="$t('OrganismoNotificato')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="far fa-dot-circle"
                ></v-combobox>

                <v-combobox
                  v-model="product.farmaco"
                  :items="medicalDevices"
                  item-text="label"
                  item-value="label"
                  :label="$t('MedicalDevice')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="fas fa-stethoscope"
                ></v-combobox>

                <v-text-field
                  v-model="product.officina"
                  :label="$t('Titolare')"
                  readonly
                  filled
                  prepend-icon="fas fa-building"
                ></v-text-field>

                <v-select
                  v-model="product.Language"
                  item-text="label"
                  item-value="id"
                  :items="languages"
                  :label="$t('Lingua')"
                  dense
                  readonly
                  filled
                  prepend-icon="fas fa-language"
                ></v-select>

                <v-text-field
                  v-model="computedDateFormattedInfo"
                  :label="$t('DataScadenza')"
                  readonly
                  filled
                  prepend-icon="fas fa-calendar-alt"
                ></v-text-field>

                <v-btn
                  v-if="!deleteAll"
                  block
                  color="error"
                  @click="deleteProduct(false)"
                  :loading="postProduct"
                  small
                  class="my-1"
                  >{{ $t("Cancella") }}</v-btn
                >

                <v-text-field
                  id="password"
                  v-show="deleteAll"
                  v-model="deleteAll_Password"
                  :label="$t('login.password')"
                  name="Password"
                  type="password"
                  color="primary"
                  :rules="passwordRules"
                  :loading="postProduct"
                  :disabled="postProduct"
                  clearable
                  autofocus
                  @keyup.enter="deleteAllProduct()"
                />

                <v-btn
                  v-if="deleteAll"
                  :disabled="deleteAll_Password.trim() == ''"
                  block
                  color="error"
                  @click="deleteAllProduct()"
                  :loading="postProduct"
                  small
                  class="my-1"
                  >Cancella definitivamente</v-btn
                >
              </v-tab-item>

              <!-- Qualifica Prodotto-->
              <v-tab-item v-if="tabVisibility(4)">
                <v-text-field
                  v-model="product.descrizione"
                  :label="$t('Descrizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-font"
                ></v-text-field>

                <v-text-field
                  v-model="product.SiglaDossier"
                  :label="$t('Edizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-code-branch"
                ></v-text-field>

                <v-combobox
                  v-model="product.dosaggio"
                  :items="notifiedEntities"
                  item-text="label"
                  item-value="label"
                  :label="$t('OrganismoNotificato')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="far fa-dot-circle"
                ></v-combobox>

                <v-combobox
                  v-model="product.farmaco"
                  :items="medicalDevices"
                  item-text="label"
                  item-value="label"
                  :label="$t('MedicalDevice')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="fas fa-stethoscope"
                ></v-combobox>

                <v-text-field
                  v-model="product.officina"
                  :label="$t('Titolare')"
                  readonly
                  filled
                  prepend-icon="fas fa-building"
                ></v-text-field>

                <v-text-field
                  v-model="output_folder"
                  :label="$t('CartellaPrincipale')"
                  :readonly="output_folder_readonly"
                  :filled="output_folder_readonly"
                  prepend-icon="fas fa-folder"
                ></v-text-field>

                <v-text-field
                  v-model="output_folder_sub"
                  :label="$t('Sottocartella')"
                  prepend-icon="fas fa-folder"
                ></v-text-field>

                <v-btn
                  block
                  color="primary"
                  @click="qProduct()"
                  :loading="postProduct"
                  small
                  class="my-1"
                  >{{ $t("Qualifica") }}</v-btn
                >
              </v-tab-item>

              <!-- Nuova edizione Prodotto -->
              <v-tab-item v-if="tabVisibility(5)">
                <v-text-field
                  v-model="formData.description"
                  :label="$t('Descrizione')"
                  dense
                  prepend-icon="fas fa-font"
                ></v-text-field>

                <v-text-field
                  v-model="formData.version"
                  :label="$t('Edizione')"
                  dense
                  prepend-icon="fas fa-code-branch"
                ></v-text-field>

                <v-combobox
                  v-model="formData.notifiedEntity"
                  :items="notifiedEntities"
                  item-text="label"
                  item-value="label"
                  :label="$t('OrganismoNotificato')"
                  loading="loadingDataContext"
                  :return-object="false"
                  prepend-icon="far fa-dot-circle"
                  dense
                ></v-combobox>

                <v-combobox
                  v-model="product.farmaco"
                  :items="medicalDevices"
                  item-text="label"
                  item-value="label"
                  :label="$t('MedicalDevice')"
                  loading="loadingDataContext"
                  readonly
                  prepend-icon="fas fa-stethoscope"
                  dense
                ></v-combobox>

                <v-text-field
                  v-model="product.officina"
                  :label="$t('Titolare')"
                  readonly
                  filled
                  prepend-icon="fas fa-building"
                  dense
                ></v-text-field>

                <v-select
                  v-model="product.Language"
                  item-text="label"
                  item-value="id"
                  :items="languages"
                  :label="$t('Lingua')"
                  readonly
                  filled
                  prepend-icon="fas fa-language"
                  dense
                ></v-select>

                <v-menu
                  v-model="formData.deadline_show"
                  :close-on-content-click="false"
                  transition="scale-transition"
                  offset-y
                  max-width="290px"
                  min-width="290px"
                >
                  <template v-slot:activator="{ on, attrs }">
                    <v-text-field
                      v-model="computedDateFormatted"
                      :label="$t('DataScadenza')"
                      prepend-icon="fas fa-calendar-alt"
                      readonly
                      v-bind="attrs"
                      v-on="on"
                    ></v-text-field>
                  </template>
                  <v-date-picker
                    v-model="formData.deadline"
                    no-title
                    @input="formData.deadline_show = false"
                  ></v-date-picker>
                </v-menu>

                <v-btn
                  block
                  color="success"
                  @click="newVersionProduct()"
                  :loading="postProduct"
                  small
                  class="my-1"
                  >{{ $t("NuovaEdizione") }}</v-btn
                >
              </v-tab-item>

              <!-- Ripristina in gestione -->
              <v-tab-item class="my-3" v-if="tabVisibility(6)">
                <v-text-field
                  v-model="CompanyName"
                  :label="$t('Azienda')"
                  readonly
                  dense
                ></v-text-field>

                <v-text-field
                  v-model="selectedDetail.productName"
                  :label="$t('Prodotto')"
                  readonly
                  dense
                ></v-text-field>

                <p>
                  <i class="fa fa-info mr-2 text-primary"></i>
                  {{ $t("ConfermaConPassword") }}
                </p>

                <v-text-field
                  id="password"
                  v-model="check_passwork"
                  :label="$t('login.password')"
                  name="Password"
                  type="password"
                  :rules="passwordRules"
                  :loading="postProduct"
                  :disabled="postProduct"
                  clearable
                  autofocus
                  @keyup.enter="rProduct()"
                />

                <v-btn
                  block
                  color="success"
                  @click="rProduct()"
                  :loading="postProduct"
                  small
                  class="my-1"
                  >{{ $t("RipristinaInGestione") }}</v-btn
                >
              </v-tab-item>

              <!-- Approvazione Technical file -->
              <v-tab-item class="my-3" v-if="tabVisibility(7)">
                <v-text-field
                  v-model="product.descrizione"
                  :label="$t('Descrizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-font"
                ></v-text-field>

                <v-text-field
                  v-model="product.SiglaDossier"
                  :label="$t('Edizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-code-branch"
                ></v-text-field>

                <v-combobox
                  v-model="product.dosaggio"
                  :items="notifiedEntities"
                  item-text="label"
                  item-value="label"
                  :label="$t('OrganismoNotificato')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="far fa-dot-circle"
                ></v-combobox>

                <v-combobox
                  v-model="product.farmaco"
                  :items="medicalDevices"
                  item-text="label"
                  item-value="label"
                  :label="$t('MedicalDevice')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="fas fa-stethoscope"
                ></v-combobox>

                <v-text-field
                  v-model="product.officina"
                  :label="$t('Titolare')"
                  readonly
                  filled
                  prepend-icon="fas fa-building"
                ></v-text-field>

                <v-text-field
                  v-model="output_folder"
                  :label="$t('CartellaPrincipale')"
                  readonly
                  :filled="output_folder_readonly"
                  prepend-icon="fas fa-folder"
                ></v-text-field>

                <v-text-field
                  v-model="output_folder_sub"
                  :label="$t('Sottocartella')"
                  readonly
                  prepend-icon="fas fa-folder"
                ></v-text-field>

                <p>
                  <i class="fa fa-info mr-2 text-primary"></i>
                  {{ $t("ConfermaConPassword") }}
                </p>

                <v-text-field
                  id="password"
                  v-model="check_passwork"
                  :label="$t('login.password')"
                  name="Password"
                  type="password"
                  :rules="passwordRules"
                  :loading="postProduct"
                  :disabled="postProduct"
                  clearable
                  autofocus
                  @keyup.enter="aProduct()"
                />

                <v-btn
                  block
                  color="success"
                  @click="aProduct()"
                  :loading="postProduct"
                  small
                  class="my-1"
                  >{{ $t("ApprovaTecnicalFile") }}</v-btn
                >
              </v-tab-item>

              <!-- ------------------------------------------------------------------------------------------------TAB DETTAGLI -->

              <!-- Nuovo  Documento ALLEGATO-->
              <v-tab-item class="my-3" v-if="tabVisibility(10)">
                <v-card flat class="h-100">
                  <v-card-text>
                    <v-text-field
                      v-model="selectedDetail.fileName"
                      :label="$t('Origine')"
                      readonly
                    ></v-text-field>

                    <v-text-field
                      v-model="formData.newTitle"
                      :label="$t('NuovoTitolo')"
                      required
                      autocomplete="off"
                      counter
                      :maxlength="selectedDetail.path_max_file_length"
                      @keyup.enter="newDocumentAttach()"
                    ></v-text-field>

                    <v-btn
                      block
                      color="primary"
                      @click="newDocumentAttach()"
                      dark
                      class="my-1"
                      :loading="postDetail"
                      >{{ $t("CreaDocumento") }}</v-btn
                    >
                  </v-card-text>
                </v-card>
              </v-tab-item>

              <!-- Proprietà -->
              <v-tab-item class="my-3" v-if="tabVisibility(11)">
                <v-text-field
                  v-model="CompanyName"
                  :label="$t('Azienda')"
                  readonly
                  dense
                ></v-text-field>

                <v-text-field
                  v-model="selectedDetail.productName"
                  :label="$t('Prodotto')"
                  readonly
                  dense
                ></v-text-field>

                <v-text-field
                  v-model="selectedDetail.fileName"
                  :label="$t('Descrizione')"
                  readonly
                  dense
                >
                  <v-icon slot="prepend" color="grey">
                    {{ filesType[selectedDetail.fileExtension.trim()] }}
                  </v-icon>
                </v-text-field>

                <v-text-field
                  v-model="selectedDetail.displayName"
                  :label="$t('Autore')"
                  readonly
                  dense
                ></v-text-field>
              </v-tab-item>

              <!-- Modifica titolo -->
              <v-tab-item class="my-3" v-if="tabVisibility(12)">
                <v-card flat class="h-100">
                  <v-card-text>
                    <v-text-field
                      v-model="selectedDetail.Title"
                      :label="$t('Descrizione')"
                      readonly
                    ></v-text-field>

                    <v-text-field
                      v-model="formData.newTitle"
                      :label="$t('NuovoTitolo')"
                      required
                      counter
                      :maxlength="selectedDetail.path_max_file_length"
                      autocomplete="off"
                      @keyup.enter="renameDocument()"
                    ></v-text-field>

                    <v-btn
                      block
                      color="primary"
                      @click="renameDocument()"
                      dark
                      class="my-1"
                      :loading="postDetail"
                      >{{ $t("RinominaDocumento") }}</v-btn
                    >
                  </v-card-text>
                </v-card>
              </v-tab-item>

              <!-- Annulla Documento -->
              <v-tab-item class="my-3" v-if="tabVisibility(13)">
                <v-card flat v-if="selDocument">
                  <v-card-text>
                    <v-text-field
                      v-model="CompanyName"
                      :label="$t('Azienda')"
                      readonly
                      filled
                    ></v-text-field>

                    <v-text-field
                      v-model="selectedDetail.productName"
                      :label="$t('Prodotto')"
                      readonly
                      filled
                    ></v-text-field>

                    <v-text-field
                      v-model="selectedDetail.fileName"
                      :label="$t('Descrizione')"
                      readonly
                      filled
                    >
                      <v-icon slot="prepend" color="grey">
                        {{ filesType[selectedDetail.fileExtension.trim()] }}
                      </v-icon>
                    </v-text-field>

                    <v-text-field
                      v-model="selectedDetail.displayName"
                      :label="$t('Autore')"
                      readonly
                      filled
                    ></v-text-field>

                    <v-btn
                      block
                      color="red"
                      @click="deleteDetail()"
                      dark
                      class="my-1"
                      :loading="postDetail"
                      >{{ $t("AnnullaDocumento") }}</v-btn
                    >
                  </v-card-text>
                </v-card>
                <p v-else>
                  Nessun prodotto selezionato
                </p>
              </v-tab-item>

              <!-- Rinomina cartella -->
              <v-tab-item class="my-3" v-if="tabVisibility(14)">
                <v-card flat class="h-100">
                  <v-card-text>
                    <v-text-field
                      v-model="formData.title"
                      :label="$t('Descrizione')"
                      readonly
                    ></v-text-field>

                    <v-text-field
                      v-model="formData.newTitle"
                      :label="$t('NuovoTitolo')"
                      required
                      autocomplete="off"
                      counter
                      :maxlength="selectedDetail.path_max_folder_length"
                      @keyup.enter="renameDocument()"
                    ></v-text-field>

                    <v-btn
                      block
                      color="success"
                      @click="renameDocument()"
                      dark
                      class="my-1"
                      :loading="postDetail"
                      >{{ $t("RinominaCartella") }}</v-btn
                    >
                    <v-divider inset />

                    <v-btn
                      v-if="isNewFolder"
                      block
                      color="red"
                      @click="deleteDetail()"
                      dark
                      class="my-1"
                      :loading="postDetail"
                      >{{ $t("AnnullaInserimentoCartella") }}</v-btn
                    >
                  </v-card-text>
                </v-card>
              </v-tab-item>

              <!-- Nuovo Documento -->
              <v-tab-item class="my-3" v-if="tabVisibility(15)">
                <v-card flat class="h-100">
                  <v-card-text>
                    <v-text-field
                      v-model="formData.title"
                      :label="$t('Destinazione')"
                      readonly
                    ></v-text-field>

                    <v-text-field
                      v-model="formData.newTitle"
                      :label="$t('NuovoTitolo')"
                      required
                      autocomplete="off"
                      counter
                      :maxlength="selectedDetail.path_max_file_length"
                      @keyup.enter="newDocument()"
                    ></v-text-field>

                    <v-btn
                      block
                      color="primary"
                      @click="newDocument()"
                      dark
                      class="my-1"
                      :loading="postDetail"
                      >{{ $t("CreaDocumento") }}</v-btn
                    >
                  </v-card-text>
                </v-card>
              </v-tab-item>

              <!-- Cancella Cartella -->
              <v-tab-item class="my-3" v-if="tabVisibility(16)">
                <v-card flat v-if="selDocument">
                  <v-card-text>
                    <v-text-field
                      v-model="CompanyName"
                      :label="$t('Azienda')"
                      readonly
                      filled
                    ></v-text-field>

                    <v-text-field
                      v-model="selectedDetail.productName"
                      :label="$t('Prodotto')"
                      readonly
                      filled
                    ></v-text-field>

                    <v-text-field
                      v-model="selectedDetail.fileName"
                      :label="$t('Destinazione')"
                      readonly
                      filled
                    >
                      <v-icon slot="prepend" color="grey">
                        {{ filesType[selectedDetail.fileExtension.trim()] }}
                      </v-icon>
                    </v-text-field>

                    <v-text-field
                      v-model="selectedDetail.displayName"
                      :label="$t('Autore')"
                      readonly
                      filled
                    ></v-text-field>

                    <v-btn
                      block
                      color="red"
                      @click="deleteDetail()"
                      dark
                      class="my-1"
                      :loading="postDetail"
                      >{{ $t("CancellaContenuto") }}</v-btn
                    >
                  </v-card-text>
                </v-card>
                <p v-else>
                  {{ $t("NessunProdottoSelezionato") }}
                </p>
              </v-tab-item>

              <!-- Note Documento -->
              <v-tab-item class="my-3" v-if="tabVisibility(17)">
                <div>
                  <h3 color="primary">{{ selectedDetail.name }}</h3>
                </div>
                <div
                  class="row"
                  v-if="!readonlyNotes && product.flag_stato == 1"
                >
                  <div class="col col-12">
                    <v-btn
                      color="green"
                      class="ma-2 white--text"
                      small
                      @click="addDetailNote()"
                    >
                      <v-icon ledt dark small class="mr-2">
                        fas fa-plus
                      </v-icon>
                      {{ $t("NuovaNota") }}
                    </v-btn>
                  </div>
                </div>
                <v-divider class="my-1"></v-divider>
                <v-skeleton-loader
                  v-if="loadNotes"
                  class="mx-auto"
                  type="list-item-avatar-three-line: avatar, paragraph"
                ></v-skeleton-loader>
                <v-timeline
                  align-top
                  dense
                  v-if="!loadNotes && notes.length > 0"
                  class="v-timeline-notes"
                >
                  <v-timeline-item
                    v-for="(iNote, index) in notes"
                    :key="index"
                    color="white"
                    :icon-color="
                      iNote.SWtiponota == 10
                        ? 'red'
                        : iNote.proprietario == idUtente
                        ? 'green'
                        : 'primary'
                    "
                    icon="fas fa-comment-alt"
                    fill-dot
                  >
                    <div>
                      <div class="font-weight-normal">
                        <strong>{{ iNote.nome_proprietario }}</strong> @{{
                          formatDateTimeInfo(iNote.dtiniziorichiesta)
                        }}
                      </div>
                      <div class="container">
                        <div class="row my-0">
                          <div
                            class="col my-0 py-0 px-0 align-top"
                            :class="
                              readonlyNotes ||
                              product.flag_stato != 1 ||
                              iNote.SWtiponota == 10
                                ? 'col-12 '
                                : 'col-8 '
                            "
                          >
                            <pre style="white-space: pre-line;">{{
                              iNote.oggetto
                            }}</pre>
                          </div>
                          <div
                            class="col col-4 my-0 py-0 px-0 align-top"
                            v-if="
                              !readonlyNotes &&
                                product.flag_stato == 1 &&
                                iNote.SWtiponota != 10
                            "
                          >
                            <v-btn icon @click="editNote(iNote)">
                              <v-icon x-small color="primary"
                                >fas fa-pen-square</v-icon
                              >
                            </v-btn>
                            <v-divider vertical inset />
                            <v-btn icon @click="deleteNote(iNote)">
                              <v-icon x-small color="red"
                                >fas fa-trash-alt</v-icon
                              >
                            </v-btn>
                          </div>
                        </div>
                      </div>
                    </div>
                  </v-timeline-item>
                </v-timeline>
              </v-tab-item>

              <!-- Nota Documento (EDIT)-->
              <v-tab-item class="my-3" v-if="tabVisibility(18)">
                <div>
                  <h3 color="primary">{{ selectedDetail.name }}</h3>
                </div>
                <v-divider class="my-1"></v-divider>
                <div>
                  <div class="row">
                    <div class="col col-6">
                      <h4>{{ $t("Autore") }}: {{ eNote.owner_name }}</h4>
                    </div>
                    <div class="col col-6"></div>
                  </div>
                  <div class="row">
                    <div class="col col-12">
                      <v-textarea
                        v-model="eNote.comment"
                        :disabled="saveNote"
                        :readonly="eNote.delete == 1"
                        class="mx-2"
                        :label="$t('NuovaNota')"
                        rows="3"
                        prepend-icon="fas fa-comment-alt"
                        color="primary"
                        dense
                      ></v-textarea>
                    </div>
                  </div>
                  <div class="row">
                    <div class="col col-6">
                      <v-btn
                        outlined
                        color="primary"
                        block
                        @click="changeTabView(7)"
                      >
                        <v-icon small class="mr-2">
                          fas fa-angle-left
                        </v-icon>
                        {{ $t("Annulla") }}
                      </v-btn>
                    </div>
                    <div class="col col-6">
                      <v-btn
                        color="success"
                        @click="saveDetailNote"
                        :loading="saveNote"
                        block
                        v-if="eNote.delete == 0"
                      >
                        {{ $t("Inserisci") }}
                      </v-btn>
                      <v-btn
                        color="error"
                        @click="saveDetailNote"
                        :loading="saveNote"
                        block
                        v-if="eNote.delete == 1"
                      >
                        {{ $t("CancellaNota") }}
                      </v-btn>
                    </div>
                  </div>
                </div>
              </v-tab-item>

              <!-- Genera revision list-->
              <v-tab-item v-if="tabVisibility(19)">
                <v-text-field
                  v-model="product.descrizione"
                  :label="$t('Descrizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-font"
                ></v-text-field>

                <v-text-field
                  v-model="product.SiglaDossier"
                  :label="$t('Edizione')"
                  readonly
                  filled
                  prepend-icon="fas fa-code-branch"
                ></v-text-field>

                <v-combobox
                  v-model="product.dosaggio"
                  :items="notifiedEntities"
                  item-text="label"
                  item-value="label"
                  :label="$t('OrganismoNotificato')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="far fa-dot-circle"
                ></v-combobox>

                <v-combobox
                  v-model="product.farmaco"
                  :items="medicalDevices"
                  item-text="label"
                  item-value="label"
                  :label="$t('MedicalDevice')"
                  :loading="loadingDataContext"
                  readonly
                  prepend-icon="fas fa-stethoscope"
                ></v-combobox>

                <v-text-field
                  v-model="product.officina"
                  :label="$t('Titolare')"
                  readonly
                  filled
                  prepend-icon="fas fa-building"
                ></v-text-field>

                <v-select
                  v-model="product.Language"
                  item-text="label"
                  item-value="id"
                  :items="languages"
                  :label="$t('Lingua')"
                  dense
                  readonly
                  filled
                  prepend-icon="fas fa-language"
                ></v-select>

                <v-text-field
                  v-model="computedDateFormattedInfo"
                  :label="$t('DataScadenza')"
                  readonly
                  filled
                  prepend-icon="fas fa-calendar-alt"
                ></v-text-field>

                <div v-for="(err, index) in errors" class="errors" :key="index">
                  {{ err }}
                </div>

                <v-btn
                  block
                  color="success"
                  @click="runRevisionList()"
                  :loading="postProduct"
                  small
                  class="my-1"
                  >{{ $t("GeneraRevisionList") }}</v-btn
                >
              </v-tab-item>

              <!-- Reperimento Documento -->
              <v-tab-item class="my-3" v-if="tabVisibility(20)">
                <div>
                  <h3 color="primary">{{ selectedDetail.name }}</h3>
                </div>

                <v-divider class="my-1"></v-divider>

                <v-skeleton-loader
                  v-if="loadPreviousDocument"
                  class="mx-auto"
                  type="list-item-avatar-three-line: avatar, paragraph"
                ></v-skeleton-loader>

                <!-- Elenco delle precedenti versioni.. previousDocuments -->
                <v-list>
                  <v-list-item
                    v-for="(fitem, index) in previousDocuments"
                    :key="index"
                  >
                    <v-list-item-avatar>
                      <v-icon
                        :color="
                          fitem.ext.trim() == 'pdf'
                            ? '#B71C1C'
                            : fitem.ext.trim() == 'docx'
                            ? '#295391'
                            : fitem.ext.trim() == 'xlsx'
                            ? '#1C6D42'
                            : '#BDBDBD'
                        "
                        >{{ files[fitem.ext.trim()] }}</v-icon
                      >
                    </v-list-item-avatar>

                    <v-list-item-content
                      @click="viewPreviousDocVer(fitem.ver, fitem.name)"
                    >
                      <v-list-item-title>
                        <strong class="text-primary"
                          >{{ $t("Versione") }}:
                        </strong>
                        {{ fitem.ver }}
                      </v-list-item-title>
                      <v-list-item-subtitle>
                        <strong class="text-primary">{{ $t("Del") }}: </strong>
                        {{ formatDateTimeInfo(fitem.dt_mod) }}
                        <v-progress-linear
                          indeterminate
                          color="primary"
                          v-if="loadPreviousDocVer == fitem.ver"
                        ></v-progress-linear>
                      </v-list-item-subtitle>
                    </v-list-item-content>

                    <v-list-item-action>
                      <v-tooltip
                        bottom
                        color="primary"
                        v-if="previousDocVer == fitem.ver"
                      >
                        <template v-slot:activator="{ on, attrs }">
                          <v-btn
                            v-bind="attrs"
                            v-on="on"
                            icon
                            @click="restorePreviousDoc(fitem.ver)"
                          >
                            <v-icon color="error">fa fa-history</v-icon>
                          </v-btn>
                        </template>
                        <span>{{ $t("RipristinaDocumento") }}</span>
                      </v-tooltip>
                    </v-list-item-action>
                  </v-list-item>
                </v-list>
              </v-tab-item>

              <!-- Upload multiplo -->
              <v-tab-item class="my-3" v-if="tabVisibility(21)">
                <div>
                  <h3 color="primary">{{ selectedDetail.name }}</h3>
                </div>

                <v-divider class="my-1"></v-divider>

                <v-list subheader three-line>
                  <v-list-item
                    v-for="(item, index) in uploadFileList"
                    :key="index"
                  >
                    <v-list-item-avatar>
                      <v-icon
                        :color="
                          item.status == 2
                            ? 'success'
                            : item.status == 1
                            ? 'warning'
                            : item.status == -1
                            ? 'error'
                            : 'grey'
                        "
                        dark
                      >
                        fas fa-file-upload
                      </v-icon>
                    </v-list-item-avatar>

                    <v-list-item-content>
                      <v-list-item-title
                        v-text="item.name + ' ( ' + item.file.size + ' )'"
                      ></v-list-item-title>

                      <v-list-item-subtitle>
                        <v-progress-linear
                          :value="item.upload"
                          color="light-green"
                          height="25"
                          v-show="item.status == 0 || item.status == 1"
                          :indeterminate="item.upload == 100"
                        >
                          <strong>{{ item.upload }} %</strong>
                        </v-progress-linear>
                      </v-list-item-subtitle>
                      <v-list-item-subtitle>
                        <div
                          class=" text--light"
                          v-for="(msg, index) in item.messages"
                          :key="index"
                        >
                          {{ msg }}
                        </div>
                      </v-list-item-subtitle>
                    </v-list-item-content>

                    <v-list-item-action>
                      <v-btn
                        icon
                        v-if="item.error && item.error == 'title_length'"
                        @click="rename_file(item)"
                      >
                        <v-icon color="error">fas fa-font</v-icon>
                      </v-btn>
                    </v-list-item-action>
                  </v-list-item>
                </v-list>
              </v-tab-item>
            </v-tabs-items>
          </v-card-text>

          <v-card-actions>
            <v-btn
              block
              color="secondary"
              @click="closeAction()"
              dark
              small
              class="my-1"
              :disabled="postProduct || postDetail"
              >{{ $t("Chiudi") }}</v-btn
            >
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
  name: "allActions",
  props: [
    "visibility",
    "selected-detail",
    "users",
    "idUtente",
    "selected-product",
    "upload-file-list"
  ],
  data() {
    return {
      deleteAll: false,
      deleteAll_Password: "",
      errors: [],
      loadingDataContext: false,
      popup: {
        color: "",
        mode: "",
        snackbar: false,
        text: "questa è una prova",
        timeout: 6000,
        x: null,
        y: "top",
        multiLine: true
      },
      tab: null,
      output_folder: "",
      output_folder_sub: "",
      output_folder_readonly: false,
      formData: {
        description: "",
        version: "",
        MedicalDeviceName: "",
        notifiedEntity: "",
        owner: "",
        language: 2,
        deadline: new Date().toISOString().substr(0, 10),
        dateFormatted: this.formatDate(new Date().toISOString().substr(0, 10)),
        deadline_show: false
      },
      languages: [
        { id: 1, code: "IT", label: "Italiano" },
        { id: 2, code: "EN", label: "English" }
      ],
      Detail: null,
      postDetail: false,
      postProduct: false,
      saveNote: false,
      loadNotes: false,
      notes: [],
      eNote: {},
      readonlyNotes: false,
      isNewFolder: false,
      check_passwork: "",
      previousDocuments: [],
      loadPreviousDocument: false,
      previousDocVer: 0,
      loadPreviousDocVer: 0,
      previousDocVerBtn: 0,
      notifiedEntities: [
        { code: "7890", label: "ORGANISMO 7890" },
        { code: "1234", label: "Altro" }
      ],
      medicalDevices: [
        { id: 1, code: "cerotto", label: "Cerotto" },
        { id: 2, code: "lozione", label: "Lozione" }
      ],
      product: null,
      temporary_file_present: false,
      upload_file_list: [],
      rename_uploaded_file: ""
    };
  },
  beforeCreate() {},
  beforeMount() {},
  mounted() {
    //this.clearForm();
    this.formData.owner = this.$store.getters[
      "config/workingCompany"
    ].companyName;
  },
  computed: {
    msg_temporary_file_not_present() {
      return this.$t("msg_temporary_file_not_present").replaceAll(
        "[Azienda]",
        this.workingCompany.companyName
      );
    },
    computedDateFormatted() {
      return this.formatDate(this.formData.deadline);
    },
    computedDateFormattedInfo() {
      return this.formatDateInfo(this.product.termineconsegna);
    },
    viewTab() {
      return this.visibility;
    },
    selDocument() {
      return this.selectedDetail;
    },
    CompanyName() {
      return this.userInfo.BusinessName;
    },
    selProduct() {
      return this.selectedProduct;
    },
    filesType() {
      return this.files;
    },
    errorList() {
      return this.errors;
    },
    passwordRules() {
      // this.errors = [];
      return [
        v => !!v || this.$t("login.err.password_required"),
        v =>
          (v && v.trim().length > 0) || this.$t("login.err.password_required")
      ];
    }
  },

  watch: {
    date(val) {
      console.log(val);
      this.formData.dateFormatted = this.formatDate(this.formData.deadline);
    },
    viewTab(val) {
      console.log(val);
      this.tab = val;
    },
    selProduct(val) {
      console.log(val);
      this.product = val;
    },
    selDetail(val) {
      console.log(val);
      this.Detail = val;
    },
    errorList(val) {
      if (val.length) {
        this.popup.text = val.join(",");
        this.popup.color = "#992233";
        this.popup.y = "top";
        this.popup.timeout = 10000;
        this.popup.snackbar = true;
      }
    },
    previousDocVer(val) {
      if (val === 0) {
        this.previousDocVerBtn = 0;
      } else {
        this.previousDocVerBtn = 1;
      }
    }
  },
  methods: {
    // ------------------------------------------------------------------------------Generic methods
    clearForm() {
      this.formData = {
        description: "",
        version: "",
        MedicalDeviceName: "",
        notifiedEntity: "",
        owner: "",
        farmaco: "",
        language: 2,
        deadline: new Date().toISOString().substr(0, 10),
        dateFormatted: this.formatDate(new Date().toISOString().substr(0, 10)),
        deadline_show: false
      };
      if (this.visibility == 2 || this.visibility == 4) {
        this.formData.newTitle = this.formData.title;
        console.log("clearForm\\title:" + this.formData.title);
        console.log("clearForm\\newTitle:" + this.formData.newTitle);
      }
      console.log(this.formData);
    },
    formatDate(date) {
      if (!date) return null;
      const [year, month, day] = date.split("-");
      return `${day}/${month}/${year}`;
    },
    formatDateInfo(date) {
      if (!date) return null;
      const d = date.split("T")[0];
      const [year, month, day] = d.split("-");
      return `${day}/${month}/${year}`;
    },
    parseDate(date) {
      if (!date) return null;

      const [month, day, year] = date.split("/");
      return `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}`;
    },
    tabVisibility(n) {
      var v = false;
      v = n == this.viewTab;
      // switch (this.viewTab) {
      //   case 1:
      //     v = n in [1, 2];
      //     break;
      //   case 0:
      //     v = n in [2];
      //     break;
      //   default:
      // }
      // console.log(v)
      return v;
    },
    closeAction() {
      console.log("productAction\\closeAction");
      this.$emit("chiudi");
    },
    changeTabView(n) {
      this.$emit("changeTabView", n);
    },
    viewMessage(type, message) {
      this.popup.color = type;
      this.errors.push(message);
      this.popup.snackbar = true;
    },
    viewMessages(type, message) {
      this.popup.color = type;
      for (var e in message) {
        this.errors.push(message[e]);
      }
      this.popup.snackbar = true;
    },
    resetPassword() {
      this.check_passwork = "";
    },

    // ------------------------------------------------------------------------------Product methods
    createProduct() {
      setTimeout(() => {
        this.newProduct();
      }, 300);
    },
    newProduct() {
      this.errors = [];
      this.postProduct = true;
      var p = this.formData;
      var company_id = this.$store.getters.myCompany.companyID;

      const params = {
        descrizione: p.description,
        SiglaDossier: p.version,
        Language: p.language,
        farmaco: p.MedicalDeviceName,
        dosaggio: p.notifiedEntity,
        idAzienda: company_id,
        officina: this.$store.getters.myCompany.companyName,
        termineconsegna: p.deadline
      };
      console.log(params);
      this.$axios
        .post("Actions/addProduct", params)
        .then(response => {
          this.postProduct = false;
          this.$emit("goto", response.data);
          // this.$emit("aggiorna");
          this.closeAction();
          console.log(response.data);
        })
        .catch(e => {
          this.postProduct = false;
          this.viewMessage("error", e.response.data.Message);
        });
    },
    newVersionProduct() {
      this.errors = [];
      this.postProduct = true;
      var p = this.formData;
      var company_id = this.$store.getters.myCompany.companyID;

      const params = {
        IDdossier: this.product.IDdossier,
        descrizione: p.description,
        SiglaDossier: p.version,
        Language: p.language,
        farmaco: this.product.farmaco,
        dosaggio: p.notifiedEntity,
        idAzienda: company_id,
        officina: this.$store.getters.myCompany.companyName,
        termineconsegna: p.deadline
      };
      console.log(params);
      this.$axios
        .post("Actions/newVersionProduct", params)
        .then(response => {
          this.postProduct = false;
          this.$emit("goto", response.data);
          this.closeAction();
          console.log(response.data);
        })
        .catch(e => {
          this.postProduct = false;
          this.viewMessage("error", e.response.data.Message);
        });
    },
    notifiedEntitysFilter(item, queryText) {
      const textOne = item.label.toLowerCase();
      const textTwo = item.code.toLowerCase();
      const searchText = queryText.toLowerCase();

      return (
        textOne.indexOf(searchText) > -1 || textTwo.indexOf(searchText) > -1
      );
    },
    loadProductDataContext() {
      this.$axios
        .get("Actions/ProductDataContext")
        .then(response => {
          var data = response.data;
          if (data.stato == 1) {
            this.medicalDevices = data.md;
            this.notifiedEntities = data.on;
            this.temporary_file_present = data.temporary_file_present;
          } else {
            this.viewMessage("error", data.messaggio);
          }
        })
        .catch(error => {
          this.viewMessage("error", error.response);
          //this.loadDataList();
        });
    },
    saveProduct() {
      this.errors = [];
      this.postProduct = true;
      // console.log(params);
      setTimeout(() => {
        this.saveProduct_lazy();
      }, 500);
    },
    saveProduct_lazy() {
      var p = this.formData;
      var company_id = this.$store.getters.myCompany.companyID;

      const params = {
        idDossier: this.selProduct.IDdossier,
        descrizione: p.description,
        SiglaDossier: p.version,
        dosaggio: p.notifiedEntity,
        farmaco: this.formData.MedicalDeviceName,
        Language: this.selProduct.Language,
        idAzienda: company_id,
        officina: this.$store.getters.myCompany.companyName,
        termineconsegna: p.deadline
      };
      console.log(params);
      this.$axios
        .post("Actions/saveProduct", params)
        .then(response => {
          this.postProduct = false;
          this.$emit("aggiorna");
          this.closeAction();
          console.log(response.data);
        })
        .catch(e => {
          this.postProduct = false;
          this.viewMessage("error", e.response.data.Message);
        });
    },
    setdeleteAll(value) {
      this.deleteAll = value;
    },
    setProductFolder(folderName, readonly) {
      var f = folderName.split("\\");
      this.output_folder = f[0];
      this.output_folder_sub = f[1];
      this.output_folder_readonly = readonly;
      console.log(folderName);
    },
    deleteAllProduct() {
      const params = {
        p: this.deleteAll_Password
      };
      this.$axios
        .post("Actions/check_p", params)
        .then(() => {
          this.postProduct = false;
          this.deleteProduct(true);
        })
        .catch(e => {
          this.postProduct = false;
          this.viewMessage("error", e.response.data.Message);
        });
    },
    qProduct() {
      this.postProduct = true;
      console.log("detailAction\\qProduct");
      this.errors = [];
      var folder_path = this.output_folder
        .concat("\\")
        .concat(this.output_folder_sub);
      const params = {
        id: this.product.IDdossier,
        folder: folder_path
      };
      this.$axios
        .post("Actions/qualifica", params)
        .then(response => {
          this.postProduct = false;
          this.$emit("aggiorna");
          this.$emit("viewMsg", "info", response.data);
          //this.viewMessage("info", response.data);
          this.closeAction();
        })
        .catch(e => {
          this.postProduct = false;
          this.viewMessage("error", e.response.data.Message);
        });
    },
    rProduct() {
      this.errors = [];
      this.postProduct = false;
      const params = {
        p: this.check_passwork
      };
      this.$axios
        .post("Actions/check_p", params)
        .then(() => {
          this.postProduct = false;
          this.restoreFnProduct();
        })
        .catch(e => {
          this.postProduct = false;
          if (e.response.data.ModelState) {
            this.viewMessages("error", e.response.data.ModelState);
          } else {
            this.viewMessage("error", e.response.data.Message);
          }
        });
    },
    restoreProduct() {
      //Ripristina dai cancellati
      this.errors = [];
      this.postProduct = true;

      this.$axios
        .get("Actions/restoreProduct/" + this.product.IDdossier)
        .then(response => {
          this.postProduct = false;
          console.log(response.data);
          this.$emit("aggiorna");
          this.closeAction();
        })
        .catch(e => {
          this.postProduct = false;
          if (e.response.data.ModelState) {
            this.viewMessages(
              "error",
              e.response.data.ModelState,
              this.$t("RipristinaDaCancellato")
            );
          } else {
            this.viewMessage(
              "error",
              e.response.data.Message,
              this.$t("RipristinaDaCancellato")
            );
          }
        });
    },
    restoreFnProduct() {
      // Ripristina dalla gestione
      this.loading_details_list = true;
      this.$axios
        .get("Actions/ripristina/" + this.product.IDdossier)
        .then(() => {
          this.loading_details_list = false;
          this.$emit("aggiorna");
          this.closeAction();
        })
        .catch(e => {
          this.loading_details_list = false;
          this.viewMessage("error", e.response.data.Message);
        });
    },
    setEditProduct(values) {
      console.log(values);
      this.$set(this.formData, "description", values.descrizione);
      this.$set(this.formData, "version", values.SiglaDossier);
      this.$set(this.formData, "notifiedEntity", values.dosaggio);
      this.$set(
        this.formData,
        "deadline",
        values.termineconsegna.split("T")[0]
      );
    },
    setEditFolder(values) {
      console.log(values);
      this.isNewFolder = false;
      this.$set(this.formData, "title", values.name);
      this.$set(this.formData, "newTitle", values.name);
    },
    setEditNewFolder(values) {
      console.log(values);
      this.isNewFolder = true;
      this.$set(this.formData, "title", values.name);
      this.$set(this.formData, "newTitle", values.name);
    },
    deleteProduct(all) {
      this.errors = [];
      this.postProduct = true;
      var canc = all ? "1" : "0";

      this.$axios
        .get("Actions/deleteProduct/" + this.product.IDdossier + "/" + canc)
        .then(response => {
          this.postProduct = false;
          this.$emit("products");
          this.closeAction();
          console.log(response.data);
        })
        .catch(e => {
          this.postProduct = false;
          if (e.message) {
            this.errors.push(e.message);
          } else if (e.response.data.Message) {
            this.errors.push(e.response.data.Message);
          } else {
            this.errors.push(e.responsedata.Message.toString());
          }
        });
    },
    aProduct() {
      this.errors = [];
      this.postProduct = true;
      const params = {
        p: this.check_passwork
      };
      this.$axios
        .post("Actions/check_p", params)
        .then(() => {
          this.postProduct = false;
          this.approveProduct();
        })
        .catch(e => {
          this.postProduct = false;
          if (e.response.data.ModelState) {
            this.viewMessages("error", e.response.data.ModelState);
          } else {
            this.viewMessage("error", e.response.data.Message);
          }
        });
    },
    approveProduct() {
      //Ripristina dai cancellati
      this.errors = [];
      this.postProduct = true;

      this.$axios
        .get("Actions/Approve/" + this.product.IDdossier)
        .then(response => {
          this.postProduct = false;
          console.log(response.data);
          this.$emit("aggiorna");
          this.closeAction();
        })
        .catch(e => {
          this.postProduct = false;
          if (e.response.data.ModelState) {
            this.viewMessages("error", e.response.data.ModelState);
          } else {
            this.viewMessage("error", e.response.data.Message);
          }
        });
    },

    // ------------------------------------------------------------------------------Details methods
    newDocument() {
      this.errors = [];
      this.postDetail = true;
      var p = this.formData;

      const params = {
        title: p.newTitle,
        id: this.selectedDetail.detailID
      };
      console.log(params);
      this.$axios
        .post("Actions/newDocument", params)
        .then(response => {
          console.log(response.data);
          var d = response.data;
          this.postDetail = false;
          if (d.stato > 0) {
            var data = response.data.detail;
            var idParent = response.data.idParent;
            this.$emit("ndoc", idParent, data);
            this.closeAction();
          } else {
            this.viewMessage("error", d.messaggio);
          }
        })
        .catch(e => {
          this.postDetail = false;
          this.errors.push(e.response.data.Message.toString());
        });
    },
    newDocumentAttach() {
      this.errors = [];
      this.postDetail = true;
      var p = this.formData;

      const params = {
        title: p.newTitle,
        id: this.selectedDetail.detailID
      };
      console.log(params);
      this.$axios
        .post("Actions/addDetailAttach", params)
        .then(response => {
          this.postDetail = false;
          this.$emit("aggiorna");
          this.closeAction();
          console.log(response.data);
        })
        .catch(e => {
          this.postDetail = false;
          this.errors.push(e.response.data.Message.toString());
        });
    },
    async rename_file(item) {
      console.log("rename..");
      var maxlength = item.title_length;
      this.rename_uploaded_file = item.name;
      let newName = await this.$dialog.prompt({
        text: this.$t("RinominaFile"),
        title: this.$t("RinominaFile"),
        actions: {
          false: this.$t("Cancella"),
          true: {
            text: this.$t("OK"),
            color: "primary"
          }
        },
        rules: [v => v.length >= 3 || `${this.$t("NomeAlmeno3Caratteri")}`], // vuetify's v-text-field rules prop
        textField: {
          maxlength: maxlength,
          counter: true,
          "v-model": "rename_uploaded_file"
          // Any addtional props/attrs that will be binded to v-text-field component
          //type: "password",
        }
      });
      if (newName) {
        const params = {
          title: newName,
          id: item.idProgressivo
        };
        console.log(params);
        this.$axios
          .post("Actions/renameDetail", params)
          .then(response => {
            var data = response.data;
            console.log(response.data);
            if (data.stato > 0) {
              item.status = 2;
              this.uploadFileList[item.n_file].status = 2;
              this.uploadFileList[item.n_file].error = null;
              this.uploadFileList[item.n_file].name = newName;
              this.$emit("multipleUploadRefreshList");

              this.$emit("aggiorna");
            } else {
              this.viewMessage("error", data.messaggio);
            }
          })
          .catch(e => {
            this.postDetail = false;
            this.errors.push(e.responsedata.Message.toString());
          });
      }
    },

    renameDocument() {
      this.errors = [];
      this.postDetail = true;
      var p = this.formData;

      const params = {
        title: p.newTitle,
        id: this.selectedDetail.detailID
      };
      console.log(params);
      this.$axios
        .post("detail/rename", params)
        .then(response => {
          this.postDetail = false;
          this.$emit("aggiorna");
          this.closeAction();
        })
        .catch(e => {
          this.postDetail = false;
          this.errors.push(e.response.data.Message.toString());
        });
    },
    deleteDetail() {
      this.errors = [];
      this.postDetail = true;
      var p = this.formData;

      const params = {
        title: p.title ? p.title : this.selectedDetail.Title,
        id: this.selectedDetail.detailID
      };
      this.$axios
        .post("detail/delete", params)
        .then(response => {
          this.postDetail = false;
          this.$emit("aggiorna");
          this.closeAction();
        })
        .catch(e => {
          this.postDetail = false;
          this.errors.push(e.responsedata.Message.toString());
        });
    },
    loadDetailNotes(id, readonly) {
      this.readonlyNotes = readonly;
      this.loadNotes = true;
      this.$axios
        .get("Actions/getDetailComments/" + id)
        .then(response => {
          console.log(response.data);
          var data = response.data;
          if (data.stato == 1) {
            this.$set(this, "notes", data.detailNotes);
            this.$emit("changeNotesCount", data.detailNotes.length);
          } else {
            this.errors.push(data.messaggio);
          }
          this.loadNotes = false;
        })
        .catch(e => {
          this.errors.push(e.responsedata.Message.toString());
          this.loadNotes = false;
        });
    },
    addDetailNote() {
      console.log("addDetailNote");
      var idx = this.idUtente;
      var name = this.getUserByID(idx).DisplayName;
      this.eNote = {
        IDnota: 0,
        owner_id: idx,
        owner_name: name,
        comment: "",
        id: this.selectedDetail.detailID,
        delete: 0
      };
      console.log("new tab");
      this.changeTabView(18);
    },
    editNote(item) {
      this.eNote = {
        IDnota: item.IDnota,
        owner_id: item.proprietario,
        owner_name: item.nome_proprietario,
        comment: item.oggetto,
        id: this.selectedDetail.detailID,
        delete: 0
      };
      console.log("new tab");
      this.changeTabView(18);
    },
    deleteNote(item) {
      this.eNote = {
        IDnota: item.IDnota,
        owner_id: item.proprietario,
        owner_name: item.nome_proprietario,
        comment: item.oggetto,
        id: this.selectedDetail.detailID,
        delete: 1
      };
      console.log("new tab");
      this.changeTabView(18);
    },
    getUserByID(idx) {
      return this.users.find(obj => {
        return obj.IDutente === idx;
      });
    },
    saveDetailNote() {
      this.errors = [];
      this.saveNote = true;

      const params = this.eNote;
      this.$axios
        .post("Actions/saveDetailComment", params)
        .then(() => {
          this.saveNote = false;
          this.loadDetailNotes(
            this.selectedDetail.detailID,
            this.readonlyNotes
          );
          this.changeTabView(17);
        })
        .catch(e => {
          this.saveNote = false;
          this.errors.push(e.responsedata.Message.toString());
        });
    },

    setEditDocument(values, asNew) {
      console.log(values);
      this.$set(this.formData, "title", values.name);
      if (asNew) {
        this.$set(this.formData, "newTitle", "Nuovo File");
      } else {
        this.$set(this.formData, "newTitle", values.name);
      }
    },
    runRevisionList() {
      this.errors = [];
      this.postProduct = true;
      this.previousDocVer = 0;
      var id = this.product.IDdossier;
      this.$axios
        .get("Actions/RevisionList/" + id)
        .then(response => {
          this.postProduct = false;
          this.$emit("aggiorna");
          this.$emit("viewMsg", "info", response.data);
          //this.viewMessage("info", response.data);
          this.closeAction();
        })
        .catch(e => {
          this.postProduct = false;
          if (e.response.data.Message) {
            this.errors.push(e.response.data.Message);
          } else {
            this.errors.push(e.responsedata.Message.toString());
          }
        });
    },
    loadPreviousDoc(id) {
      this.previousDocuments = [];
      this.loadPreviousDocument = true;
      this.previousDocVer = 0;
      this.$axios
        .get("Actions/getPreviousDoc/" + id)
        .then(response => {
          console.log(response.data);
          var data = response.data;
          if (data.stato == 1) {
            this.$set(this, "previousDocuments", data.documents);
          } else {
            this.errors.push(data.messaggio);
          }
          this.loadPreviousDocument = false;
        })
        .catch(e => {
          this.loadPreviousDocument = false;
          if (e.message) {
            this.errors.push(e.message);
          } else if (e.response.data.Message) {
            this.errors.push(e.response.data.Message);
          } else {
            this.errors.push(e.responsedata.Message.toString());
          }
        });
    },
    viewPreviousDocVer(ver, fileName) {
      this.errors = [];
      this.postDetail = true;
      this.previousDocVer = 0;
      this.loadPreviousDocVer = ver;
      const params = new URLSearchParams();
      params.append("ecd_file", this.product.IDdossier + "\\" + fileName);
      //params.append(headers, {'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8', 'Accept-language': this.$i18n.locale})
      this.$axios
        .get("Actions/viewPreviousDoc", { params })
        .then(() => {
          this.postDetail = false;
          setTimeout(() => {
            this.previousDocVer = ver;
            this.loadPreviousDocVer = 0;
          }, 3000);
        })
        .catch(e => {
          this.postDetail = false;
          this.loadPreviousDocVer = 0;
          this.previousDocVer = 0;
          if (e.response.data.Message) {
            this.errors.push(e.response.data.Message);
          } else {
            this.errors.push(e.responsedata.Message.toString());
          }
        });
    },
    restorePreviousDoc(ver) {
      this.loadPreviousDocument = true;
      this.$axios
        .get(
          "Actions/restorePreviousDoc/" +
            this.selectedDetail.detailID +
            "/" +
            ver
        )
        .then(response => {
          console.log(response.data);
          this.loadPreviousDocument = false;
          var data = response.data;
          this.previousDocVer = 0;
          if (data.stato == 1) {
            this.$emit("aggiorna");
            this.$emit("viewMsg", "info", data.messaggio);
            //this.viewMessage("info", response.data);
            this.closeAction();
          } else {
            this.errors.push(data.messaggio);
          }
          this.loadPreviousDocument = false;
        })
        .catch(e => {
          this.loadPreviousDocument = false;
          if (e.message) {
            this.errors.push(e.message);
          } else if (e.response.data.Message) {
            this.errors.push(e.response.data.Message);
          } else {
            this.errors.push(e.responsedata.Message.toString());
          }
        });
    }
  }
};
</script>

<style scoped>
.card-h-100 {
  height: 98vh;
}
.inner-card {
  /* height: calc(100vh - 48px);    (48 è il tab, 36 è il pulsante chiudi = 74)    */
  height: calc(100vh - 110px);
  min-height: calc(100vh - 110px);
  max-height: calc(100vh - 110px);
  overflow-y: auto;
  padding-top: 16px;
}

.v-progress-linear__bar,
.v-progress-linear__bar__determinate {
  transition: none;
}
</style>
