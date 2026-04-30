import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    /**
     * Dev: set VITE_API_BASE_URL=/api in .env and run your Web API
     * (e.g. https://localhost:7205). CORS is bypassed for /api in dev.
     * Production: use full API URL in VITE_API_BASE_URL and add CORS on the API.
     */
    proxy: {
      "/api": {
        target: "https://localhost:7205",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
