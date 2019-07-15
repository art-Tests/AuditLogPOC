﻿window.eventBus = new Vue({});

const refId = 100270010
const logType = "contract"

const app = new Vue({
  el: "#app",
  data: {
    emptyForm: {
      refId: refId,
      logType: logType,
      beginDate: null,
      endDate:null,
      name: null
    },
    defaultForm: {
        refId: refId,
        logType: logType,
        beginDate: "2018-08-01",
        endDate:"2020-07-31",
        name: "內湖王陽明"
    },
    form: {
        refId: refId,
        logType: logType,
        beginDate: null,
        endDate:null,
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
          alert("insert ok");
          vm.updateDataIntoDB({ ...vm.form });
          window.eventBus.$emit("getLogs", 1);
        } else {
          alert("something wrong!!");
        }
      });
    },
    deleteLog() {
      $.ajax({
        url: `http://localhost:54117/API/AuditLog`,
        type: "DELETE"
      }).done(function(res) {
        if (res === "OK") {
          alert("clear ok");
          window.eventBus.$emit("getLogs", 1);
        }
      });
    }
  }
});

const log = new Vue({
  el: "#log",
  data: {
    records: null,
    form: {
      page: 1,
      pageSize: 10,
      logType: logType
    },
    status: {
      totalCount: null
    }
  },
  mounted() {
    this.getAuditLog();
    window.eventBus.$on("sendQuery", this.sendQuery);
    window.eventBus.$on("getLogs", this.sendQuery);
  },
  methods: {
    recordNumber(index) {
      return (this.form.page - 1) * this.form.pageSize + index;
    },
    getAuditLog() {
      var vm = this;
      $.ajax({
        url: `http://localhost:54117/API/AuditLog?logType=${vm.form.logType}&page=${vm.form.page}&size=${vm.form.pageSize}`
      }).done(function(response) {
        vm.records = response.Logs;
        vm.status.totalCount = response.TotalCount;
      });
    },

    sendQuery(page) {
      this.form.page = page;
      this.getAuditLog();
    }
  },
  computed: {
    noRecords() {
      return this.filterRecords.length === 0;
    },
    filterRecords() {
      if (this.records) {
        if (this.form.page <= this.maxPage) {
          return this.records.slice(0, this.form.pageSize);
        }
      }
      return [];
    },
    totalCount() {
      return this.status.totalCount;
    },
    pagingStart() {
      //第幾筆開始 = 分頁筆數 * ( 目前頁數 - 1 ) + 1
      return this.form.page >= 1 ? this.form.pageSize * (this.form.page - 1) + 1 : 1;
    },
    pagingEnd() {
      return this.form.page >= 1
        ? this.pagingStart + this.form.pageSize - 1
        : this.totalCount >= this.form.pageSize
        ? this.form.pageSize
        : this.totalCount;
    },
    pagingTotal() {
      return this.totalCount;
    },
    nowPage() {
      return this.form.page;
    },
    maxPage() {
      let totalCount = this.totalCount;
      let pageSize = this.form.pageSize;
      let maxPage = totalCount % pageSize === 0 ? parseInt(totalCount / pageSize) : parseInt(totalCount / pageSize) + 1;
      return maxPage;
    }
  }
});