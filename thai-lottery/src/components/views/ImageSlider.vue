<template>
  <div class="slider-container">
    <div ref="swiperEl" class="swiper">
      <div class="swiper-wrapper">
        <div class="swiper-slide">
          <img src="../../assets/slider-ticket.jpg" alt="Pile of lottery tickets" />
        </div>
        <div class="swiper-slide">
          <img src="../../assets/slider-lottery.jpg" alt="Colorful numbered lottery balls" />
        </div>
        <div class="swiper-slide">
          <img src="../../assets/slider-winner.jpg" alt="Winner raising both fists in victory" />
        </div>
      </div>
      <div class="swiper-pagination"></div>
    </div>
  </div>
</template>

<script>
import Swiper, { Autoplay, Pagination } from 'swiper'
import 'swiper/swiper-bundle.css'

export default {
  name: 'ImageSlider',
  mounted() {
    this.swiper = new Swiper(this.$refs.swiperEl, {
      modules: [Autoplay, Pagination],
      loop: true,
      autoplay: {
        delay: 4000
      },
      pagination: {
        el: '.swiper-pagination',
        clickable: true
      }
    })
  },
  beforeUnmount() {
    if (this.swiper) this.swiper.destroy()
  }
}
</script>

<style scoped>
.slider-container {
  border-radius: 8px;
  overflow: hidden;
}

.swiper {
  width: 100%;
  height: 220px;
}

.swiper-slide img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

@media (max-width: 767px) {
  .swiper {
    height: 140px;
  }
}
</style>
