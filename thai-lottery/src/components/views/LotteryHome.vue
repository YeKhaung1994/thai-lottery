<template>
  <div>
    <div class="logo-container">
      <img class="logo" src="../../assets/logo.png" alt="Logo" />
    </div>
    <h1>Welcome to the Thailand Lottery</h1>
    <!-- <ImageSlider/> -->
    <br/><br/><br/>

    <div class="tabs-container">
      <div
        v-for="(tab, index) in tabs"
        :key="index"
        class="tab"
        :class="{ active: activeTab === tab }"
        @click="setActiveTab(tab)"
        :style="{ width: `${100 / tabs.length}%` }"
      >
        {{ tab }}
      </div>
    </div>

    <div v-if="activeTab === 'Results'">
      <LotteryResults />
    </div>

    <div v-if="activeTab === 'Winners'">
      <LotteryWinners />
    </div>

    <div v-if="activeTab === 'Tickets'">
      <LotteryTickets />
      <!-- Add content for Tickets tab -->
    </div>

    <div v-if="activeTab === 'AboutUs'">
      <AboutUs />
    </div>

    <LotteryFooter />
    <!-- Add other content here -->
  </div>
</template>

<script>
import LotteryResults from "./LotteryResults.vue";
import LotteryWinners from "./LotteryWinners.vue";
import LotteryFooter from "./LotteryFooter.vue";
import LotteryTickets from "./LotteryTickets.vue";
import AboutUs from "./AboutUs.vue";
// import ImageSlider from "./ImageSlider.vue";

export default {
  name: "LotteryHome",
  components: {
    LotteryResults,
    LotteryWinners,
    LotteryTickets,
    AboutUs,
    LotteryFooter,
    // ImageSlider,
  },
  data() {
    return {
      activeTab: "Results",
      tabs: ["Results", "Winners", "Tickets", "AboutUs"], // Add your desired tabs here
    };
  },
  methods: {
    setActiveTab(tab) {
      this.activeTab = tab;
    },
  },
};
</script>

<style scoped>
/* Add custom styles for the Home component here */

.tabs-container {
  display: flex;
  justify-content: space-between;
  margin-bottom: 20px;
}

.tab {
  padding: 10px;
  cursor: pointer;
  font-weight: bold;
  text-align: center;
  border-bottom: 2px solid transparent;
  transition: border-color 0.3s;
}

.tab.active {
  border-color: #000;
  background-color: #f5f5f5;
  color: #000;
}

/* Example styles for the tabs */
.tab:hover {
  background-color: #f5f5f5;
  color: #000;
}

.logo-container {
  position: absolute;
  top: 20px;
  left: 80px;
}

.logo {
  width: 100px; /* Set the desired width of the logo */
  height: auto;
}

/* Add more styles as needed for the LotteryResults and LotteryWinners components */
</style>
