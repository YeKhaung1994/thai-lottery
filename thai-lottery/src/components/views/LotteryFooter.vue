<template>
  <footer class="footer">
    <p>© {{ year }} ထောပြီ — unofficial Thai lottery results viewer. Data: Government Lottery Office (glo.or.th).</p>
    <nav class="footer-nav" aria-label="Footer">
      <router-link to="/results">Results</router-link>
      <router-link to="/winners">Winners</router-link>
      <router-link to="/about">About</router-link>
    </nav>
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

.footer-nav {
  display: flex;
  gap: 20px;
}

.footer-nav a {
  color: #6b6b6b;
  text-decoration: none;
  min-height: 44px;
  display: inline-flex;
  align-items: center;
}

.footer-nav a:hover {
  color: #d97706;
}

@media (max-width: 767px) {
  .footer {
    padding: 20px 16px 90px 16px;
    justify-content: center;
    text-align: center;
  }
}
</style>
