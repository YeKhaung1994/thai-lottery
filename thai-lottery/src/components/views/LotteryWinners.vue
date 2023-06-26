<template>
  <div class="lottery-winners">
    <h1>Lottery Winners</h1>
    <div class="winner-list">
      <div class="card" v-for="(category, categoryIndex) in winners.prizes" :key="categoryIndex">
         <h3 class="card-title">{{ category.name }}</h3>
         <span class="card-subtitle">{{ category.number.length }}</span>
       <div class="card-body" v-if="flag">
          <div v-for="(subcategory, subcategoryIndex) in category.number" :key="subcategoryIndex">
          <p class="card-text">{{ subcategory }}</p>
          <!-- <ul>
            <li v-for="(item, itemIndex) in subcategory.items" :key="itemIndex">
              {{ item.name }} - {{ item.price }}
            </li>
          </ul> -->
        </div>
      </div>
     
    </div>
    <!-- <div class="card" v-for="winner in winners" :key="winner.id">
      <div class="card-header">
        <h3 class="card-title">{{ winner.name }}</h3>
        <span class="card-subtitle">{{ winner.date }}</span>
      </div>
      <div class="card-body">
        <p class="card-text">{{ winner.prize }}</p>
        <p class="card-text">{{ winner.location }}</p>
      </div>
    </div> -->
  </div>
  </div>
</template>

<script>
import axios from 'axios';
export default {
  name: "LotteryWinners",
  data() {
    return {
      winners: [],
      flag: false,
      // winners: [
      //   {
      //     id: 1,
      //     position: "1st",
      //     name: "John Doe",
      //     prize: "$10,000",
      //     image: "path/to/winner1.jpg",
      //   },
      //   {
      //     id: 2,
      //     position: "2nd",
      //     name: "Jane Smith",
      //     prize: "$5,000",
      //     image: "path/to/winner2.jpg",
      //   },
      //   // Add more winner objects as needed
      // ],
    };  
  },
    methods: {
    fetchData() {
      axios.get('https://lotto.api.rayriffy.com/latest')
        .then(response => {
          this.winners = response.data.response;
          // Handle the response data
          console.log(response.data);
        })
        .catch(error => {
          // Handle any errors
          console.error(error);
        });
    }
  },
  created() {
    this.fetchData(); // Call the API when the component is created
  }
};
</script>

<style scoped>
.lottery-winners {
  text-align: center;
}

h1 {
  font-size: 24px;
  margin-bottom: 20px;
}

.winners-list {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
}

.winner {
  width: 80%;
  margin: 10px;
  padding: 10px;
  background-color: #f5f5f5;
  border-radius: 10px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.winner-info {
  margin-bottom: 10px;
}

.position {
  font-weight: bold;
  font-size: 18px;
}

.name {
  font-size: 16px;
}

.prize {
  font-size: 14px;
}

.winner-image img {
  width: 100%;
  height: auto;
  border-radius: 5px;
}

@media (max-width: 767px) {
  .winner {
    width: 100%;
  }
}

.winner-list {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
}

.card {
  background-color: #f5f5f5;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  width: 300px;
}

.card-header {
  padding: 20px;
  border-bottom: 1px solid #ddd;
}

.card-title {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
}

.card-subtitle {
  font-size: 14px;
  color: #777;
}

.card-body {
  padding: 20px;
}

.card-text {
  margin: 0;
  font-size: 14px;
  line-height: 1.5;
}
</style>
