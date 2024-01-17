/* eslint-env node:true */
import react from '@vitejs/plugin-react'
import path from 'path'
import { fileURLToPath, URL } from 'url'
import { defineConfig } from 'vite'
import eslint from 'vite-plugin-eslint'

export default defineConfig({
  css: {
    devSourcemap: true
  },
  // eslint-disable-next-line no-undef
  base: process.env.NODE_ENV === 'production' ? '/briefcases/' : '/',
  plugins: [react(), eslint()],
  build: {
    outDir: path.resolve('..', 'wwwroot', 'briefcases'),
    emptyOutDir: true,
    rollupOptions: {
      // eslint-disable-next-line no-undef
      input: path.join(__dirname, 'src', 'main.jsx')
    }
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      src: '/src'
    }
  },
  server: {
    hmr: {
      protocol: 'ws'
    }
  }
})
