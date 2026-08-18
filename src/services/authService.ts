import { api } from './api';
import type { LoginRequest, TokenResponse } from '../types/auth.types';

export const authService = {
  login: async (data: LoginRequest): Promise<TokenResponse> => {
    const response = await api.post<TokenResponse>('/auth/login', data);
    return response.data;
  },
};
