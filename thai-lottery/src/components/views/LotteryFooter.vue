<template>
  <footer class="footer">
    <p>© {{ year }} ထောပြီ — unofficial Thai lottery results viewer. Data: Government Lottery Office (glo.or.th).</p>
  </footer>
</template>

<script>
export default {
  name: 'LotteryFooter',
  computed: {
    year() {
      return new Date().getFullYear()
    }
  }
}
</script>

<style scoped>
.footer {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 12px 24px;
  padding: 20px 48px;
  border-top: 1px solid var(--line);
  font-size: 14px;
  color: var(--muted);
}

.footer p {
  margin: 0;
}

@media (max-width: 767px) {
  .footer {
    padding: 20px 16px 90px 16px;
    justify-content: center;
    text-align: center;
  }
}
</style>
