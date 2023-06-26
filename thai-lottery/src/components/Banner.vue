<template>
  <div class="banner">
    <h2>{{ title }}</h2>
    <p>{{ message }}</p>
  </div>
</template>

<script>
export default {
  props: {
    title: {
      type: String,
      required: true,
    },
    message: {
      type: String,
      required: true,
    },
  },
};
</script>

<style scoped>
.banner {
  background-color: #ffcc00;
  color: #fff;
  padding: 20px;
  text-align: center;
}
</style>
