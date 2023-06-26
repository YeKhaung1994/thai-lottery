<template>
  <div class="lottery-results">
    <h1>Lottery Results</h1>
    
    <div class="filter-container">
      <label for="search">Search:</label>
      <input type="text" id="search" v-model="searchQuery" placeholder="Enter a keyword" />
      
      <label for="date">Filter by Date:</label>
      <input type="date" id="date" v-model="selectedDate" />
    </div>

    <div class="results-container">
      <div class="result-item" v-for="result in filteredResults" :key="result.id">
        <div class="result-number">{{ result.number }}</div>
        <div class="result-prize">Prize: ${{ result.prize }}</div>
        <div class="result-date">Date: {{ result.date }}</div>
      </div>
    </div>
  </div>
</template>

<script>


export default {
  name: "LotteryResults",
  data() {
    return {
      searchQuery: "",
      selectedDate: null,
      results: [
        { id: 1, number: 1, prize: 100, date: "2023-06-01" },
        { id: 2, number: 2, prize: 50, date: "2023-06-05" },
        { id: 3, number: 3, prize: 200, date: "2023-06-10" },
        // Add more result objects with unique date values
      ],
    };
  },
  computed: {
    filteredResults() {
      let filteredResults = this.results;
      
      if (this.searchQuery) {
        const lowercaseQuery = this.searchQuery.toLowerCase();
        filteredResults = filteredResults.filter(result =>
          result.number.toString().includes(lowercaseQuery)
        );
      }
      
      if (this.selectedDate) {
        filteredResults = filteredResults.filter(result =>
          result.date === this.selectedDate
        );
      }
      
      return filteredResults;
    },
  },
};
</script>

<style scoped>
/* Add custom styles for the LotteryResults component here */

.filter-container {
  margin-bottom: 20px;
}

.results-container {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  margin-top: 20px;
}

.result-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 150px;
  height: 150px;
  margin: 10px;
  border: 1px solid #ccc;
  border-radius: 8px;
  background-color: #f5f5f5;
}

.result-number {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 10px;
}

.result-prize {
  font-size: 16px;
  color: #888;
}

.result-date {
  font-size: 14px;
  color: #888;
}
</style>
