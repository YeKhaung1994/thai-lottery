<template>
  <div class="data-table">
    <div class="table-meta">
      <span>{{ rows.length }} {{ noun }}{{ rows.length === 1 ? '' : 's' }}</span>
      <slot name="meta" />
    </div>
    <div class="table-scroll">
      <table>
        <thead>
          <tr>
            <th
              v-for="col in columns"
              :key="col.key"
              :class="[col.align, { sortable: col.sortable }]"
              :aria-sort="sortKey === col.key ? (sortDir === 1 ? 'ascending' : 'descending') : 'none'"
            >
              <button v-if="col.sortable" type="button" class="sort-button" @click="sortBy(col.key)">
                {{ col.label }}
                <span v-if="sortKey === col.key" class="sort-arrow">{{ sortDir === 1 ? '▲' : '▼' }}</span>
              </button>
              <template v-else>{{ col.label }}</template>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in pageRows" :key="rowKey(row)">
            <td v-for="col in columns" :key="col.key" :class="col.align">
              <slot :name="`cell-${col.key}`" :row="row">{{ row[col.key] }}</slot>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-if="pageCount > 1" class="table-pager">
      <button type="button" :disabled="page === 1" @click="page -= 1">‹ Prev</button>
      <span>Page {{ page }} of {{ pageCount }}</span>
      <button type="button" :disabled="page === pageCount" @click="page += 1">Next ›</button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DataTable',
  props: {
    /** [{ key, label, align: 'right'|null, sortable: Boolean }] */
    columns: {
      type: Array,
      required: true
    },
    rows: {
      type: Array,
      required: true
    },
    idKey: {
      type: String,
      default: 'id'
    },
    noun: {
      type: String,
      default: 'row'
    },
    pageSize: {
      type: Number,
      default: 25
    }
  },
  data() {
    return { sortKey: null, sortDir: 1, page: 1 }
  },
  computed: {
    sorted() {
      if (!this.sortKey) return this.rows
      const key = this.sortKey
      const dir = this.sortDir
      return [...this.rows].sort((a, b) => {
        const x = a[key]
        const y = b[key]
        if (typeof x === 'number' && typeof y === 'number') return (x - y) * dir
        return String(x).localeCompare(String(y)) * dir
      })
    },
    pageCount() {
      return Math.max(1, Math.ceil(this.sorted.length / this.pageSize))
    },
    pageRows() {
      const start = (this.page - 1) * this.pageSize
      return this.sorted.slice(start, start + this.pageSize)
    }
  },
  watch: {
    rows() {
      this.page = 1
    }
  },
  methods: {
    rowKey(row) {
      return row[this.idKey] ?? JSON.stringify(row)
    },
    sortBy(key) {
      if (this.sortKey === key) {
        this.sortDir *= -1
      } else {
        this.sortKey = key
        this.sortDir = 1
      }
      this.page = 1
    }
  }
}
</script>

<style scoped>
.data-table {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.table-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  font-size: 14px;
  font-weight: 600;
  color: var(--muted);
}

.table-scroll {
  overflow-x: auto;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}

th,
td {
  padding: 10px 16px;
  border-bottom: 1px solid var(--line);
  text-align: left;
  white-space: nowrap;
}

th {
  position: sticky;
  top: 0;
  background: var(--amber-tint);
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  z-index: 1;
}

tbody tr {
  height: 40px;
}

tbody tr:hover {
  background: var(--cream);
}

tbody tr:last-child td {
  border-bottom: none;
}

th.right,
td.right {
  text-align: right;
  font-variant-numeric: tabular-nums;
}

.sort-button {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  border: none;
  background: none;
  font: inherit;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 700;
  cursor: pointer;
  padding: 0;
  min-height: 24px;
}

.sort-arrow {
  font-size: 9px;
}

.table-pager {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  font-size: 14px;
}

.table-pager button {
  min-height: 40px;
  padding: 0 14px;
  border: 1px solid var(--line-strong);
  border-radius: 8px;
  background: var(--card);
  font: inherit;
  font-weight: 600;
  cursor: pointer;
}

.table-pager button:hover:not(:disabled) {
  border-color: var(--amber);
}

.table-pager button:disabled {
  color: #c9c9c9;
  cursor: not-allowed;
}
</style>
