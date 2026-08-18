import { api } from './api';
import type { CatalogWordDetail, CatalogWordsPage } from '../types/catalogWord.types';
import type { CatalogWordStats } from '../types/catalogWordStats.types';

export const catalogWordService = {
  listByCategoryAndLetter: async (
    categorySlug: string, letter: string, page: number, pageSize: number, seed: number,
  ): Promise<CatalogWordsPage> => {
    const response = await api.get<CatalogWordsPage>(`/catalog/categories/${categorySlug}/words`, {
      params: { page, pageSize, letter, seed },
    });
    return response.data;
  },
  getBySlug: async (slug: string): Promise<CatalogWordDetail> => {
    const response = await api.get<CatalogWordDetail>(`/catalog/words/${slug}`);
    return response.data;
  },
  getWordStats: async (): Promise<CatalogWordStats> => {
    const response = await api.get<CatalogWordStats>('/catalog/words/stats');
    return response.data;
  },
};
