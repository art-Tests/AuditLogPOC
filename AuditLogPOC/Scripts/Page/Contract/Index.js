window.eventBus = new Vue({});

const config = {
  logType: "contract",
  refId: 100270010,
  delayMs: 1000,
  logTable: {
    el: "#log"
  }
};

const app = new Vue({
  el: "#app",
  data: {
    emptyForm: {
      refId: config.refId,
      logType: config.logType,
      beginDate: null,
      endDate: null,
      name: null
    },
    defaultForm: {
      refId: config.refId,
      logType: config.logType,
      beginDate: "2018-08-01",
      endDate: "2020-07-31",
      name: "內湖王陽明"
    },
    form: {
      refId: config.refId,
      logType: config.logType,
      beginDate: null,
      endDate: null,
      name: null
    }
  },
  mounted() {
    this.getDataFromDB();
  },
  methods: {
    checkForm() {
      for (const key in this.form) {
        if (this.form.hasOwnProperty(key)) {
          const element = this.form[key];
          if (element) return true;
        }
      }
      return false;
    },
    clearForm() {
      this.form = { ...this.emptyForm };
    },
    getDataFromDB() {
      this.form = { ...this.defaultForm };
    },
    updateDataIntoDB(formData) {
      this.defaultForm = { ...formData }; // 模擬更新資料回DB
    },
    delay(ms) {
      return new Promise(resolve => {
        setTimeout(resolve, ms);
      });
    },
    createLog() {
      if (this.checkForm() === false) {
        console.log("plz input data in form");
        return;
      }
      var vm = this;
      // 此處為了Demo才傳遞舊資料
      let sendData = {
        refId: vm.form.refId,
        logType: vm.form.logType,
        newForm: JSON.stringify({ ...vm.form }),
        oldForm: JSON.stringify({ ...vm.defaultForm })
      };

      $.ajax({
        url: `http://localhost:54117/API/AuditLog`,
        data: sendData,
        type: "POST"
      }).done(function(res) {
        if (res === "OK") {
          vm.updateDataIntoDB({ ...vm.form });
          vm.delay(config.delayMs).then(() => {
            window.eventBus.$emit("getLogs", 1);
          });
        } else {
          alert("something wrong!!");
        }
      });
    },
    deleteLog() {
      $.ajax({
        url: `http://localhost:54117/API/Reset?logType=${config.logType}`,
        type: "POST"
      }).done(function(res) {
        if (res === "OK") {
          alert("clear ok");
          window.eventBus.$emit("getLogs", 1);
        }
      });
    }
  }
});
