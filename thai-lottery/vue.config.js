const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    // History-mode router: serve index.html for deep links like /winners.
    historyApiFallback: true,
    proxy: {
      // GLO sends no CORS headers, so the browser can't call it directly.
      '/glo': {
        target: 'https://www.glo.or.th',
        changeOrigin: true,
        pathRewrite: { '^/glo': '' }
      }
    }
  }
})
