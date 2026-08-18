import axios, { type InternalAxiosRequestConfig } from 'axios';

export const TOKEN_STORAGE_KEY = 'adedonha.accessToken';

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export function attachAuthHeader(config: InternalAxiosRequestConfig): InternalAxiosRequestConfig {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}

api.interceptors.request.use(attachAuthHeader);

export function resolveUploadUrl(path: string | null): string | null {
  if (!path) return null;
  try {
    const origin = new URL(import.meta.env.VITE_API_BASE_URL).origin;
    return `${origin}${path}`;
  } catch {
    return path;
  }
}
