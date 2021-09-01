export default {
  //Definizione-----------------
  components: {}, //Elenco di componenti utilizzati
  data: () => ({
    uploadAcceptType:
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/pdf, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    files: {
      html: "fa fa-file-code",
      doc: "fas fa-file-word",
      docx: "fas fa-file-word",
      json: "fa fa-js-square",
      pdf: "fas fa-file-pdf",
      png: "fa fa-file-image",
      jpg: "fa fa-file-image",
      gif: "fa fa-file-image",
      txt: "fas fa-file-alt",
      xls: "fas fa-file-excel",
      xlsx: "fas fa-file-excel",
      zip: "fa fa-file-archive",
      rar: "fa fa-file-archive",
      "7z": "fa fa-file-archive",
      xml : "far fa-file-code"
    },

  }), // i dati definiscono un oggetto che rappresenta i dati interni del componente Vue. Può anche essere una funzione che restituisce l'oggetto dati.
  methods: {
    async loadConfig() {
      console.log("loadConfig");
      this.configLoading = true;
      try {
        var config = (await this.$axios.get("appconfig")).data;
        // for (var p in config) {
        //   this.appConfig[config[p].Parameter] = config[p].Value;
        //   console.log("Parametro acquisito: " + config[p].Parameter)
        // }
        this.$store.commit("config/updateAppConfig", { Items: config });
      } catch (error) {
        this.configLoading = false;
        this.viewMessageError(error, "APP Configuration");
      }
      this.configLoading = false;
    },
    async login() {
      this.loginProgess = true;
      this.msg_err_login = "";
      const params = {
        UserName: this.usr,
        NewPassword: this.user_password_new,
        password: this.pwd
      };
      await this.$axios
        .post("login", params)
        .then(response => {
          this.loginProgess = false;
          var r = response.data;
          if (r.stato < 0) {
            // Errore login
            this.msg_err_login = r.messaggio;
          } else {
            this.appMenu = r.appMenu;
            this.userInfo = r.userInfo;
            var pg = "/" + this.userInfo.area + "/dashboard";
            console.log(pg);
            this.$router.push(pg);
          }
        })
        .catch(e => {
          this.loginProgess = false;
          this.viewMessage("error", e.response.data.Message);
        });
    },
    async saveUser() {
      this.user.progress = true;
      const params = {
        userID: this.user.userInfo.userID,
        UserName: this.user.userInfo.UserName,
        password: this.user.userInfo.password,
        DisplayName: this.user.userInfo.DisplayName,
        email: this.user.userInfo.email,
        companyID: this.company.companyInfo.companyID,
      };
      console.log("saveUser");
      await this.$axios
        .post("saveUser", params)
        .then(response => {
          this.user.progress = false;
          var r = response.data;
          if (r.stato < 0) {
            // Errore login
            this.viewMessageError(response);
          } else {
            //var Info = r.userInfo;
            this.viewMessage("success", r.messaggio, "SAVE USER");
            this.user.dialog = false;
            this.loadDataList();
          }
        })
        .catch(e => {
          this.user.progress = false;
          this.viewMessageError(e);
        });
    },
    async saveCompany() {
      this.user.progress = true;
      const params = {
        companyID: this.company.companyInfo.companyID,
        BusinessName: this.company.companyInfo.BusinessName,
        companyRoleID: this.company.companyInfo.companyRoleID,
        companyRoleName: this.company.companyInfo.companyRoleName,
        SRN: this.company.companyInfo.SRN,
        country: this.company.companyInfo.country,
        details: {users:[]}
      };
      console.log("saveCompany");
      await this.$axios
        .post("saveCompany", params)
        .then(response => {
          this.user.progress = false;
          var r = response.data;
          if (r.stato < 0) {
            // Errore login
            this.viewMessageError(response);
          } else {
            //var Info = r.userInfo;
            this.viewMessage("success", r.messaggio, "SAVE COMPANY");
            this.company.dialog = false;
            this.loadDataList();
          }
        })
        .catch(e => {
          this.user.progress = false;
          this.viewMessageError(e);
        });
    },
    viewMessageError(error, title) {
      var msg = "Errore generico";
      var timeout = 2000;
      console.log("log errore");
      if (error) {
        if (error.response) {
          if (error.response.data) {
            if (error.response.data.Message) {
              //error.response.data.Message
              msg = error.response.data.Message;
            }
            if (error.response.data.ModelState) {
              // var m = [];
              // var t = 0;
              // for (var n in error.response.data.ModelState[""]) {
              //   //error.response.data.ModelState
              //   m.push(error.response.data.ModelState[""][n]);
              //   t++;
              // }
              var msgs = this.ModelStateMessage(error.response.data.ModelState);
              msg = msgs.join("\n\n");
              timeout = timeout * msgs.length;
            }
          }
        } else if (error.data && error.data.ModelState) {
          var msgs = this.ModelStateMessage(error.data.ModelState);
          msg = msgs.join("\n\n");
          timeout = timeout * msgs.length;
        } else if (error.Message) {
          //error.Message
          msg = error.Message;
        } else if (error.message) {
          //error.message
          msg = error.message;
        }
      } else {
        msg = "Errore non gestito";
      }

      this.$snotify.error(msg, title, {
        timeout: timeout,
        showProgressBar: true,
        closeOnClick: true
      });
    },
    ModelStateMessage(ms) {
      var m = [];
      for (var n in ms) {
        //error.response.data.ModelState
        m.push(ms[n]);
      }
      return m;
    },
    viewMessage(type, message, title) {
      switch (type) {
        case "success":
          this.$snotify.success(message, title, {
            timeout: 2000,
            showProgressBar: true,
            closeOnClick: true
          });
          break;
        case "error":
          this.$snotify.error(message, title, {
            timeout: 5000,
            showProgressBar: true,
            closeOnClick: false,
            pauseOnHover: true
          });
          break;
      }
    },
    permitted(v) {
      // var p =
      //   v == "" ? true : this.$store.state.userData.permissionsList.includes(v);
      var p = true;
      console.log("permitted (" + v + ") : " + p);
      return p;
    },

  }, //l'oggetto metodi contiene una coppia chiave-valore di nomi di metodo e la relativa definizione di funzione. Questi fanno parte del comportamento del componente Vue che l'altro componente può attivare.
  computed: {
    lng: {
      get() {
        return this.$i18n.locale;
      },
      set(v){
        return this.$i18n.setLocale(v)
      }
    },
    appConfig: {
      // getter
      get: function() {
        console.log("global\\computed\\appConfig\\get..");
        console.log(this.$store.getters);
        //return this.$store.getters.appConfig;
        return this.$store.state.config.appConfig;
      },
      // setter
      set: function(newValue) {
        this.$store.commit("config/updateAppConfig", { Items: newValue });
        //this.$store.state.contatti.Items = newValue
      }
    },
    configLoading: {
      // getter
      get: function() {
        console.log("mixins\\global\\computed\\configLoading\\get");
        return this.$store.state.config.appLoading;
      },
      // setter
      set: function(newValue) {
        console.log("mixins\\global\\computed\\configLoading\\set");
        this.$store.commit("config/updateAppLoading", { val: newValue });
      }
    },
    userInfo: {
      // getter
      get: function() {
        console.log("mixins\\global\\computed\\userInfo\\get");
        return this.$store.state.config.userInfo;
      },
      // setter
      set: function(newValues) {
        console.log("mixins\\global\\computed\\userInfo\\set");
        this.$store.commit("config/updateUserInfo", { data: newValues });
      }
    },
    appMenu: {
      // getter
      get: function() {
        console.log("mixins\\global\\computed\\appMenu\\get");
        return this.$store.state.config.appMenu;
      },
      // setter
      set: function(newValues) {
        console.log("mixins\\global\\computed\\appMenu\\set");
        this.$store.commit("config/updatAppMenu", { data: newValues });
      }
    },
    availableLocales() {
      return this.$i18n.locales.filter(i => i.code !== this.$i18n.locale);
    },
    language() {
      return this.$i18n.locales.filter(i => i.code == this.$i18n.locale)[0]
        .name;
    }
  }, // contiene un oggetto che definisce le funzioni getter e setter per le proprietà calcolate del componente Vue. Le proprietà calcolate influenzano un aggiornamento reattivo sul DOM ogni volta che il loro valore cambia.
  props: {}, //contiene un array o un oggetto di proprietà specifiche del componente Vue.js, impostato al momento dell'invocazione.
  watch: {}, // questo oggetto tiene traccia dei cambiamenti nel valore di una qualsiasi delle proprietà definite come parte dei "dati" impostando le funzioni per controllarli.
  //Eventi------------------------
  beforeCreate() {}, //questo è il primo stato del ciclo di vita. Non puoi ancora interagire con nessuna parte del componente
  created() {}, //questo è subito dopo la creazione dell'istanza del componente. Ora puoi interagire con il componente, ad es. le proprietà dei dati, i watcher, le proprietà calcolate, ma non puoi ancora accedere al DOM. Di solito, i dati vengono recuperati dal database o dall'API in questo hook del ciclo di vita.
  beforeMount() {}, //il componente è compilato in questa fase, ma deve ancora essere visualizzato sullo schermo.
  mounted() {}, //questo avviene dopo che il componente è stato montato. Ora puoi accedere al metodo $ el e giocare con il contenuto all'interno degli elementi HTML. In questa fase il componente diventa completamente interattivo.
  beforeUpdate() {}, //ogni volta che vengono apportate modifiche ai dati o al DOM, subito prima, viene chiamato questo hook del ciclo di vita. Ciò è utile quando è necessario registrare le modifiche.
  updated() {}, //subito dopo che sono state apportate le modifiche al DOM o ai dati. Qui puoi eseguire operazioni dipendenti dalla modifica nel DOM.
  beforeDestroy() {}, //questo è subito prima che il componente venga distrutto ed è l'ultima istanza del DOM completamente funzionante. Puoi eseguire le operazioni di chiusura necessarie.
  destroyed() {} //questo è un po 'simile all'hook beforeCreate in cui il componente non è funzionale e non è possibile accedere a proprietà dei dati, watcher, proprietà calcolate ed eventi.
};
