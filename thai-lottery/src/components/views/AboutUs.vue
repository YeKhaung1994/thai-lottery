<template>
  <div class="about">
    <h1>About</h1>
    <p>
      ထောပြီ is an unofficial viewer for Thailand Government Lottery results. It shows the
      latest draw, past results, and winning numbers, and checks your tickets against every
      prize category of any draw.
    </p>
    <p>
      Results are fetched live from the official
      <a href="https://www.glo.or.th" target="_blank" rel="noopener">Government Lottery Office (GLO)</a>
      — draws take place on the <strong>1st and 16th of each month</strong>, usually announced
      from around 14:30 (Thailand time).
    </p>

    <h2>Prize structure</h2>
    <div class="table-wrap">
      <table>
        <thead>
          <tr>
            <th>Prize</th>
            <th>How to win</th>
            <th>Winning numbers per draw</th>
            <th>Reward per ticket</th>
          </tr>
        </thead>
        <tbody>
          <tr><td>1st Prize</td><td>Match all 6 digits</td><td>1</td><td>฿6,000,000</td></tr>
          <tr><td>Adjacent to 1st</td><td>One above or below the 1st prize</td><td>2</td><td>฿100,000</td></tr>
          <tr><td>2nd Prize</td><td>Match all 6 digits</td><td>5</td><td>฿200,000</td></tr>
          <tr><td>3rd Prize</td><td>Match all 6 digits</td><td>10</td><td>฿80,000</td></tr>
          <tr><td>4th Prize</td><td>Match all 6 digits</td><td>50</td><td>฿40,000</td></tr>
          <tr><td>5th Prize</td><td>Match all 6 digits</td><td>100</td><td>฿20,000</td></tr>
          <tr><td>3-Digit Front</td><td>Match the first 3 digits</td><td>2</td><td>฿4,000</td></tr>
          <tr><td>3-Digit Back</td><td>Match the last 3 digits</td><td>2</td><td>฿4,000</td></tr>
          <tr><td>2-Digit</td><td>Match the last 2 digits</td><td>1</td><td>฿2,000</td></tr>
        </tbody>
      </table>
    </div>
    <p class="note">One ticket can win more than one running-number prize at the same time.</p>

    <h2>Claiming a prize</h2>
    <p>
      Winning tickets can be claimed for up to <strong>2 years</strong> from the draw date, at
      GLO headquarters in Bangkok or at authorized banks. Prizes are subject to a 0.5% stamp
      duty (government lottery). Bring the physical ticket and your ID.
    </p>

    <p class="disclaimer">
      This site is not affiliated with the GLO. Always verify winning tickets against the
      official published results before claiming a prize.
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
  max-width: 720px;
  text-align: left;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.about h1 {
  margin: 0;
  font-size: 28px;
}

.about h2 {
  margin: 14px 0 0 0;
  font-size: 21px;
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

.table-wrap {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 15px;
}

th,
td {
  padding: 10px 14px;
  border: 1px solid #e2e2e2;
  text-align: left;
  white-space: nowrap;
}

th {
  background: #faf7f2;
  font-weight: 700;
}

td:last-child {
  font-variant-numeric: tabular-nums;
  font-weight: 600;
}

.note {
  font-size: 14px;
  color: #6b6b6b;
}

.disclaimer {
  color: #6b6b6b;
  font-size: 14px;
}
</style>
