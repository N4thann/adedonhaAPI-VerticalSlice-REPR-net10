import { isAxiosError } from 'axios';

export function extractApiErrorMessage(error: unknown, fallback: string): string {
  if (isAxiosError(error) && typeof error.response?.data?.title === 'string') {
    return error.response.data.title;
  }
  return fallback;
}
