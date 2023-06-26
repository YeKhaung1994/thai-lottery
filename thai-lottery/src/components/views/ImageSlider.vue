<template>
  <div>
    <div class="slider-container">
      <div class="swiper-container">
        <div class="swiper-wrapper">
          <div class="swiper-slide">
            <img src="../../assets/1.png" alt="Image 1" />
          </div>
          <div class="swiper-slide">
            <img src="../../assets/2.png" alt="Image 2" />
          </div>
          <div class="swiper-slide">
            <img src="../../assets/3.png" alt="Image 3" />
          </div>
          <!-- Add more slides as needed -->
        </div>
        <div class="swiper-pagination"></div>
      </div>
    </div>

    <!-- Rest of the component -->
  </div>
</template>

<script>
import Swiper from 'swiper';
import 'swiper/swiper-bundle.css'; // Import Swiper styles
//import 'swiper/css/swiper.css';

export default {
  name: 'ImageSlider',
  mounted() {
    new Swiper('.swiper-container', {
      loop: true,
      //speed: 1000, // Set the transition speed in milliseconds (1 second)
      autoplay: {
        delay: 1000, // Set the delay between slide transitions in milliseconds (1 second)
      },
      pagination: {
        el: '.swiper-pagination',
        clickable: true,
      },
    });
  },
};
</script>

<style scoped>
/* Add custom styles for the Home component here */

.slider-container {
  width: 100%;
  height: 100px; /* Set the desired height for your slider */
}

.swiper-slide img {
  width: 100%;
  height: 150px;
  object-fit: cover;
}
</style>
