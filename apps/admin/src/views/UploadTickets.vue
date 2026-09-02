<template>
  <div class="upload">
    <h1>Upload Tickets</h1>

    <section class="card">
      <h2>Single ticket</h2>
      <p class="hint">Tickets can only be uploaded for the next draw: <strong>{{ nextDraw }}</strong>.</p>
      <form class="single-form" @submit.prevent="submitSingle">
        <label>
          <span>Draw date (next draw only)</span>
          <input v-model="single.drawDate" type="text" :placeholder="nextDraw" pattern="\d{4}-\d{2}-\d{2}" required />
        </label>
        <label>
          <span>Number</span>
          <input v-model.trim="single.number" type="text" inputmode="numeric" maxlength="6" pattern="\d{6}" required />
        </label>
        <label>
          <span>Price (฿)</span>
          <input v-model.number="single.price" type="number" min="1" step="0.01" required />
        </label>
        <BaseButton type="submit" :disabled="busy">Add</BaseButton>
      </form>
    </section>

    <section class="card">
      <h2>CSV bulk upload</h2>
      <p class="hint">One line per ticket: <code>drawDate,number,price</code> (e.g. <code>2026-09-16,417212,120</code>). A header row is skipped automatically.</p>
      <div
        class="dropzone"
        :class="{ over: dragOver }"
        @dragover.prevent="dragOver = true"
        @dragleave="dragOver = false"
        @drop.prevent="onDrop"
      >
        <AppIcon name="ticket" :size="24" />
        <span>Drag a .csv file here, or</span>
        <label class="file-label">
          browse…
          <input type="file" accept=".csv,text/csv" class="sr-only" @change="onFile" />
        </label>
      </div>
      <textarea
        v-model="csv"
        rows="6"
        :aria-label="'CSV rows'"
        placeholder="2026-09-16,417212,120&#10;2026-09-16,888888,300"
        @input="preview = null"
      ></textarea>
      <div class="csv-actions">
        <BaseButton variant="secondary" :disabled="!csv.trim()" @click="buildPreview">Preview</BaseButton>
        <BaseButton v-if="preview && preview.valid.length" :disabled="busy" @click="commitPreview">
          {{ busy ? 'Uploading…' : `Upload ${preview.valid.length} valid row${preview.valid.length === 1 ? '' : 's'}` }}
        </BaseButton>
      </div>
    </section>

    <section v-if="preview" class="card">
      <h2>Preview</h2>
      <p class="hint">
        {{ preview.valid.length }} valid · {{ preview.invalid.length }} invalid — only valid rows are uploaded.
      </p>
      <div class="table-wrap">
        <table>
          <thead>
            <tr><th></th><th>Row</th><th>Draw date</th><th>Number</th><th>Price</th><th>Problem</th></tr>
          </thead>
          <tbody>
            <tr v-for="row in preview.all" :key="row.line" :class="row.error ? 'bad' : 'good'">
              <td>{{ row.error ? '✗' : '✓' }}</td>
              <td>{{ row.line }}</td>
              <td>{{ row.drawDate }}</td>
              <td>{{ row.number }}</td>
              <td>{{ row.price }}</td>
              <td>{{ row.error || '' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>

    <p v-if="error" class="status error" role="alert">{{ error }}</p>
    <section v-if="report" class="card">
      <h2>Server result</h2>
      <p class="ok">{{ report.inserted }} ticket{{ report.inserted === 1 ? '' : 's' }} inserted.</p>
      <div v-if="report.rejected.length" class="table-wrap">
        <table>
          <thead>
            <tr><th>Row</th><th>Number</th><th>Error</th></tr>
          </thead>
          <tbody>
            <tr v-for="r in report.rejected" :key="r.row + r.number" class="bad">
              <td>{{ r.row }}</td><td>{{ r.number }}</td><td>{{ r.error }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  </div>
</template>

<script>
import { AppIcon, BaseButton, useToasts } from '@htawpyi/shared-ui'
import { uploadTickets } from '@/services/adminApi'

const ISO = /^\d{4}-\d{2}-\d{2}$/

// Draws are on the 1st and 16th, Thailand time — the only uploadable draw.
function nextDrawIso() {
  const today = new Date().toLocaleDateString('en-CA', { timeZone: 'Asia/Bangkok' })
  const [y, m, d] = today.split('-').map(Number)
  const next = Number(d) < 16 ? new Date(y, m - 1, 16) : new Date(y, m, 1)
  return next.toLocaleDateString('en-CA')
}

export default {
  name: 'UploadTickets',
  components: { AppIcon, BaseButton },
  setup() {
    const { push } = useToasts()
    return { toast: push }
  },
  data() {
    return {
      nextDraw: nextDrawIso(),
      single: { drawDate: nextDrawIso(), number: '', price: null },
      csv: '',
      preview: null,
      report: null,
      error: null,
      busy: false,
      dragOver: false
    }
  },
  methods: {
    onDrop(event) {
      this.dragOver = false
      const file = event.dataTransfer.files?.[0]
      if (file) this.readFile(file)
    },
    onFile(event) {
      const file = event.target.files?.[0]
      if (file) this.readFile(file)
      event.target.value = ''
    },
    readFile(file) {
      const reader = new FileReader()
      reader.onload = () => {
        this.csv = String(reader.result || '').trim()
        this.preview = null
        this.buildPreview()
      }
      reader.readAsText(file)
    },
    parseRows() {
      return this.csv.split('\n')
        .map((line) => line.trim())
        .filter((line) => line && !/^drawdate/i.test(line))
        .map((line, i) => {
          const [drawDate, number, price] = line.split(',').map((v) => (v || '').trim())
          return { line: i + 1, drawDate, number, price }
        })
    },
    buildPreview() {
      const seen = new Set()
      const all = this.parseRows().map((row) => {
        let error = null
        if (!ISO.test(row.drawDate)) error = 'Draw date must be yyyy-MM-dd'
        else if (row.drawDate !== this.nextDraw) error = `Only the next draw (${this.nextDraw}) can be uploaded`
        else if (!/^\d{6}$/.test(row.number)) error = 'Number must be exactly 6 digits'
        else if (!(Number(row.price) > 0)) error = 'Price must be a positive number'
        else if (seen.has(`${row.drawDate},${row.number}`)) error = 'Duplicate in this file'
        else seen.add(`${row.drawDate},${row.number}`)
        return { ...row, error }
      })
      this.preview = {
        all,
        valid: all.filter((r) => !r.error),
        invalid: all.filter((r) => r.error)
      }
      this.report = null
    },
    async submitSingle() {
      await this.send([{ ...this.single }])
      if (!this.error) {
        this.toast(`Ticket ${this.single.number} added.`, 'success')
        this.single = { drawDate: this.single.drawDate, number: '', price: this.single.price }
      }
    },
    async commitPreview() {
      const rows = this.preview.valid.map((r) => ({
        drawDate: r.drawDate, number: r.number, price: Number(r.price)
      }))
      await this.send(rows)
      if (!this.error && this.report) {
        this.toast(`${this.report.inserted} tickets uploaded.`, 'success')
        if (!this.report.rejected.length) {
          this.csv = ''
          this.preview = null
        }
      }
    },
    async send(rows) {
      this.busy = true
      this.error = null
      this.report = null
      try {
        this.report = await uploadTickets(rows)
      } catch (err) {
        this.error = err.message
      } finally {
        this.busy = false
      }
    }
  }
}
</script>

<style scoped>
.upload {
  display: flex;
  flex-direction: column;
  gap: 18px;
  text-align: left;
  max-width: 820px;
}

h1 {
  margin: 0;
  font-size: 26px;
}

.card {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 20px 24px;
  background: var(--card);
  border: 1px solid var(--line);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
}

.card h2 {
  margin: 0;
  font-size: 18px;
}

.hint {
  margin: 0;
  font-size: 14px;
  color: var(--muted);
}

.hint code {
  background: var(--cream);
  padding: 1px 6px;
  border-radius: 4px;
  font-family: var(--font-mono);
  font-size: 13px;
}

.single-form {
  display: flex;
  flex-wrap: wrap;
  align-items: flex-end;
  gap: 12px;
}

label {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 13px;
  font-weight: 600;
  color: var(--muted);
}

input,
textarea {
  min-height: 44px;
  padding: 8px 12px;
  border: 1px solid var(--line);
  border-radius: 8px;
  font: inherit;
  font-size: 15px;
  box-sizing: border-box;
}

textarea {
  font-family: var(--font-mono);
  font-size: 13px;
  resize: vertical;
}

input:focus,
textarea:focus {
  outline: 2px solid var(--amber);
  outline-offset: 1px;
}

.dropzone {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  min-height: 84px;
  border: 2px dashed var(--line-strong);
  border-radius: var(--radius);
  background: var(--cream);
  color: var(--muted);
  font-size: 15px;
}

.dropzone.over {
  border-color: var(--amber);
  background: var(--amber-tint);
  color: var(--amber-dark);
}

.file-label {
  display: inline-flex;
  align-items: center;
  min-height: 44px;
  padding: 0 6px;
  color: var(--amber-dark);
  font-weight: 700;
  cursor: pointer;
}

.csv-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.status.error {
  margin: 0;
  color: var(--danger);
  font-weight: 600;
}

.ok {
  margin: 0;
  font-weight: 600;
  color: var(--success);
}

.table-wrap {
  overflow-x: auto;
}

table {
  border-collapse: collapse;
  font-size: 14px;
  width: 100%;
}

th,
td {
  padding: 8px 12px;
  border: 1px solid var(--line);
  text-align: left;
  white-space: nowrap;
}

th {
  background: var(--amber-tint);
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

tr.good td:first-child {
  color: var(--success);
  font-weight: 700;
}

tr.bad td {
  background: var(--danger-tint);
}

tr.bad td:first-child {
  color: var(--danger);
  font-weight: 700;
}

.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  overflow: hidden;
  clip: rect(0 0 0 0);
  white-space: nowrap;
}
</style>
