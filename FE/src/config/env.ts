import { z } from "zod";

function boolish(defaultVal: boolean) {
  return z
    .string()
    .optional()
    .transform((v) => {
      if (v === undefined || v === "") return defaultVal;
      if (v === "true" || v === "1") return true;
      if (v === "false" || v === "0") return false;
      return defaultVal;
    });
}

const envSchema = z.object({
  VITE_API_BASE_URL: z.string().min(1).default("/api"),
  VITE_MOCK_API: boolish(false),
  VITE_OIDC_AUTHORITY: z.string().default(""),
  VITE_OIDC_CLIENT_ID: z.string().default(""),
  VITE_OIDC_REDIRECT_URI: z.string().default(""),
  VITE_OIDC_POST_LOGOUT_REDIRECT_URI: z.string().default(""),
  VITE_OIDC_SCOPE: z.string().default("openid profile offline_access bla-interview-api"),
  VITE_OIDC_REGISTRATION_URL: z.string().default(""),
  VITE_USE_HOSTED_REGISTRATION: boolish(true),
  VITE_REGISTRATION_API_URL: z.string().default(""),
});

export type AppEnv = z.infer<typeof envSchema>;
export const env: AppEnv = envSchema.parse({ ...import.meta.env });
export const isMockApi = env.VITE_MOCK_API;
