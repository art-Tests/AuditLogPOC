Vue.component("log-form", {
  props: {
    defaultForm: Object
  },

  methods:{
    deleteLog(){
        console.log('del log')
    }
  },
  template: `
    <div>
        <div>
            模擬 DB 資料:{{ defaultForm }}
            <button @@click.prevent="deleteLog">清除所有異動紀錄</button>
        </div>
        <hr />
        <form>
            
            <slot></slot>

            <button @@click.prevent="createLog">修改</button>
        </form>
    </div>
    `
});

Vue.component("paging-info", {
  props: {
    start: Number,
    end: Number,
    total: Number,
    maxPage: {
      default: 1,
      type: Number
    },
    nowPage: {
      default: 1,
      type: Number
    }
  },
  methods: {
    sendQuery(page) {
      window.eventBus.$emit("sendQuery", page);
    },
    sendQueryLast() {
      let page = parseInt(this.nowPage) - 1;
      window.eventBus.$emit("sendQuery", page);
    },
    sendQueryNext() {
      let page = parseInt(this.nowPage) + 1;
      window.eventBus.$emit("sendQuery", page);
    }
  },
  computed: {
    page() {
      return this.nowPage;
    },
    pages() {
      const GetNearNumber = (last, next) => (last + 1 === next - 1 ? last + 1 : 0);
      const GetMiddleNumber = (page, maxPage) => {
        let mid;
        if (page <= 4) {
          mid = this.minPage + this.range;
        } else if (4 <= page && page < maxPage - this.range) {
          mid = page;
        } else {
          mid = maxPage - this.range;
        }
        return mid;
      };
      let count = this.maxPage <= 6 ? this.maxPage : 7;
      let result = Array.from(Array(count).keys(), x => x + 1);
      if (count <= 6) return result;

      let mid = GetMiddleNumber(this.page, this.maxPage);
      result[0] = this.minPage;
      result[1] = GetNearNumber(this.minPage, mid - 1);
      result[2] = mid - 1;
      result[3] = mid;
      result[4] = mid + 1;
      result[5] = GetNearNumber(mid + 1, this.maxPage);
      result[6] = this.maxPage;
      return result;
    },
    enableLast() {
      if (this.maxPage <= 1 || this.page <= 1) return false;
      return true;
    },
    enableNext() {
      if (this.page >= this.maxPage) return false;
      return true;
    },
    begin() {
      return this.total < this.start ? 0 : this.start;
    },
    over() {
      if (this.total >= this.end) {
        return this.end;
      } else {
        return this.total;
      }
    }
  },
  data() {
    return {
      minPage: 1,
      range: 3
    };
  },

  template: `
<div class="d-flex justify-content-between mt-3">
    <div class="dataTables_info" id="searchKeywordTable_info" role="status" aria-live="polite">
        顯示第 {{ begin }} 至 {{ over }} 項結果，共 {{ this.total }} 項
    </div>

    <ul class="pagination" v-cloak>
        <li class="paginate_button page-item previous" v-if="enableLast">
            <a href="javascript:void(0)" class="page-link" @click.prevent="sendQueryLast">上頁</a>
        </li>

        <li class="paginate_button page-item"
            :class="[nowPage == page ? 'active': '' , page==0 ? 'disabled' : '' ]"
            v-for="(page,index) in pages"
            :key="index">
            <a href="javascript:void(0)" class="page-link" @click.prevent="sendQuery(page)">{{ page }}</a>
        </li>

        <li class="paginate_button page-item next" v-if="enableNext">
            <a href="javascript:void(0)" class="page-link" @click.prevent="sendQueryNext">下頁</a>
        </li>
    </ul>
</div>`
});
