import { api } from './api';
import type { PaginatedResult } from '../types/common.types';
import type { Category, CategoryCreatePayload, CategoryUpdatePayload } from '../types/category.types';

export const categoryService = {
  list: async (page: number, pageSize: number, search?: string): Promise<PaginatedResult<Category>> => {
    const response = await api.get<PaginatedResult<Category>>('/admin/categories', { params: { page, pageSize, search } });
    return response.data;
  },
  getById: async (id: string): Promise<Category> => {
    const response = await api.get<Category>(`/admin/categories/${id}`);
    return response.data;
  },
  create: async (payload: CategoryCreatePayload): Promise<Category> => {
    const response = await api.post<Category>('/admin/categories', payload);
    return response.data;
  },
  update: async (id: string, payload: CategoryUpdatePayload): Promise<Category> => {
    const response = await api.put<Category>(`/admin/categories/${id}`, payload);
    return response.data;
  },
  remove: async (id: string): Promise<void> => {
    await api.delete(`/admin/categories/${id}`);
  },
};
