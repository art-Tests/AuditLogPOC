new Vue({
  el: "#log",
  data: {
    records: null,
    form: {
      page: 1,
      pageSize: 10,
      logType: config.logType
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
