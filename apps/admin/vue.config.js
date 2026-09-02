const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    historyApiFallback: true,
    proxy: {
      '/api': {
        target: process.env.PLATFORM_API_TARGET || 'http://localhost:5210',
        changeOrigin: true
      }
    }
  }
})
