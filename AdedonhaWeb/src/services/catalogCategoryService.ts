import { api } from './api';
import type { CatalogCategoryDetail, CatalogCategorySummary } from '../types/catalogCategory.types';
import type { CategoryWordCount } from '../types/categoryWordCount.types';

export const catalogCategoryService = {
  listMural: async (): Promise<CatalogCategorySummary[]> => {
    const response = await api.get<{ categories: CatalogCategorySummary[] }>('/catalog/categories');
    return response.data.categories;
  },
  getBySlug: async (slug: string): Promise<CatalogCategoryDetail> => {
    const response = await api.get<CatalogCategoryDetail>(`/catalog/categories/${slug}`);
    return response.data;
  },
  getWordCounts: async (): Promise<CategoryWordCount[]> => {
    const response = await api.get<{ items: CategoryWordCount[] }>('/catalog/categories/word-counts');
    return response.data.items;
  },
};
