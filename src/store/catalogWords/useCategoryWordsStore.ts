import { create } from 'zustand';
import type { CatalogWordListItem } from '../../types/catalogWord.types';
import { catalogWordService } from '../../services/catalogWordService';

interface CategoryWordsState {
  words: CatalogWordListItem[];
  page: number;
  pageSize: number;
  seed: number;
  hasMore: boolean;
  isLoading: boolean;
  error: string | null;
  initialize: (categorySlug: string, letter: string, pageSize: number) => Promise<void>;
  loadNextPage: (categorySlug: string, letter: string) => Promise<void>;
}

export const useCategoryWordsStore = create<CategoryWordsState>((set, get) => ({
  words: [], page: 0, pageSize: 10, seed: 0, hasMore: true, isLoading: false, error: null,

  initialize: async (categorySlug, letter, pageSize) => {
    const seed = Math.floor(Math.random() * 2147483647);
    set({ words: [], page: 1, pageSize, seed, hasMore: true, isLoading: true, error: null });
    try {
      const result = await catalogWordService.listByCategoryAndLetter(categorySlug, letter, 1, pageSize, seed);
      set({ words: result.items, hasMore: result.items.length === pageSize, isLoading: false });
    } catch {
      set({ error: 'Erro ao carregar palavras.', isLoading: false });
    }
  },

  loadNextPage: async (categorySlug, letter) => {
    const { page, pageSize, seed, isLoading, hasMore, words } = get();
    if (isLoading || !hasMore) return;

    const nextPage = page + 1;
    set({ isLoading: true, error: null });
    try {
      const result = await catalogWordService.listByCategoryAndLetter(categorySlug, letter, nextPage, pageSize, seed);
      set({ words: [...words, ...result.items], page: nextPage, hasMore: result.items.length === pageSize, isLoading: false });
    } catch {
      set({ error: 'Erro ao carregar mais palavras.', isLoading: false });
    }
  },
}));
