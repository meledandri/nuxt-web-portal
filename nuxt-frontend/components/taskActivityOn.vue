<template>
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
</template>


<script>
export default {
    //Definizione-----------------
  name: "component-activity",
//   components: {},        //Elenco di componenti utilizzati
  props: [
    "company",
  ],
  data: () => ({}),     // i dati definiscono un oggetto che rappresenta i dati interni del componente Vue. Può anche essere una funzione che restituisce l'oggetto dati.
  methods: {
  },  //l'oggetto metodi contiene una coppia chiave-valore di nomi di metodo e la relativa definizione di funzione. Questi fanno parte del comportamento del componente Vue che l'altro componente può attivare.
  computed: {},         // contiene un oggetto che definisce le funzioni getter e setter per le proprietà calcolate del componente Vue. Le proprietà calcolate influenzano un aggiornamento reattivo sul DOM ogni volta che il loro valore cambia.  
  props:{},             //contiene un array o un oggetto di proprietà specifiche del componente Vue.js, impostato al momento dell'invocazione. 
  watch:{},             // questo oggetto tiene traccia dei cambiamenti nel valore di una qualsiasi delle proprietà definite come parte dei "dati" impostando le funzioni per controllarli.
    //Eventi------------------------
  beforeCreate() {},    //questo è il primo stato del ciclo di vita. Non puoi ancora interagire con nessuna parte del componente
  created () {},    //questo è subito dopo la creazione dell'istanza del componente. Ora puoi interagire con il componente, ad es. le proprietà dei dati, i watcher, le proprietà calcolate, ma non puoi ancora accedere al DOM. Di solito, i dati vengono recuperati dal database o dall'API in questo hook del ciclo di vita.
  beforeMount () {},    //il componente è compilato in questa fase, ma deve ancora essere visualizzato sullo schermo.
  mounted () {},    //questo avviene dopo che il componente è stato montato. Ora puoi accedere al metodo $ el e giocare con il contenuto all'interno degli elementi HTML. In questa fase il componente diventa completamente interattivo.
  beforeUpdate () {},    //ogni volta che vengono apportate modifiche ai dati o al DOM, subito prima, viene chiamato questo hook del ciclo di vita. Ciò è utile quando è necessario registrare le modifiche.
  updated () {},    //subito dopo che sono state apportate le modifiche al DOM o ai dati. Qui puoi eseguire operazioni dipendenti dalla modifica nel DOM.
  beforeDestroy () {},    //questo è subito prima che il componente venga distrutto ed è l'ultima istanza del DOM completamente funzionante. Puoi eseguire le operazioni di chiusura necessarie.
  destroyed () {},    //questo è un po 'simile all'hook beforeCreate in cui il componente non è funzionale e non è possibile accedere a proprietà dei dati, watcher, proprietà calcolate ed eventi. 
};
</script>
