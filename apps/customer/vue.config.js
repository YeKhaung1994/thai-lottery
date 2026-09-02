const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    // History-mode router: serve index.html for deep links like /winners.
    historyApiFallback: true,
    proxy: {
      // GLO sends no CORS headers, so the browser can't call it directly.
      // Target is overridable via GLO_PROXY_TARGET in .env.local.
      '/glo': {
        target: process.env.GLO_PROXY_TARGET || 'https://www.glo.or.th',
        changeOrigin: true,
        pathRewrite: { '^/glo': '' }
      },
      // Platform API (apps/api).
      '/api': {
        target: process.env.PLATFORM_API_TARGET || 'http://localhost:5210',
        changeOrigin: true
      }
    }
  }
})
