<template>
  <div class="about">
    <h1>About</h1>
    <p>
      This site is an unofficial viewer for Thailand Government Lottery results. It shows the
      latest draw, past results, and winning numbers, and lets you check a ticket against every
      prize category of the most recent draw.
    </p>
    <p>
      Results are fetched live from the official
      <a href="https://www.glo.or.th" target="_blank" rel="noopener">Government Lottery Office (GLO)</a>
      — draws take place on the 1st and 16th of each month.
    </p>
    <p class="disclaimer">
      This site is not affiliated with the GLO. Always verify winning tickets against the official
      published results before claiming a prize.
    </p>
  </div>
</template>

<script>
export default {
  name: 'AboutUs'
}
</script>

<style scoped>
.about {
  max-width: 640px;
  text-align: left;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.about h1 {
  margin: 0;
  font-size: 28px;
}

.about p {
  margin: 0;
  font-size: 16px;
  line-height: 1.6;
}

.about a {
  color: #b45309;
}

.about a:hover {
  color: #92400e;
}

.disclaimer {
  color: #6b6b6b;
  font-size: 14px;
}
</style>
