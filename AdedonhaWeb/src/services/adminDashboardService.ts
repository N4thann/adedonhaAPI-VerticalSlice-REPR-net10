import { api } from './api';
import type { CategoryWordCount } from '../types/categoryWordCount.types';
import type { WordStats } from '../types/wordStats.types';

export const adminDashboardService = {
  getCategoryWordCounts: async (): Promise<CategoryWordCount[]> => {
    const response = await api.get<{ items: CategoryWordCount[] }>('/admin/categories/word-counts');
    return response.data.items;
  },
  getWordStats: async (): Promise<WordStats> => {
    const response = await api.get<WordStats>('/admin/words/stats');
    return response.data;
  },
};
