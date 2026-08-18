import { api } from './api';
import type { CategoryWordCount } from '../types/categoryWordCount.types';

export const adminDashboardService = {
  getCategoryWordCounts: async (): Promise<CategoryWordCount[]> => {
    const response = await api.get<{ items: CategoryWordCount[] }>('/admin/categories/word-counts');
    return response.data.items;
  },
};
