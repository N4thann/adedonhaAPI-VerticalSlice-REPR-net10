import { create } from 'zustand';
import type { WordCreatePayload, WordListItem, WordUpdatePayload } from '../../types/word.types';
import { wordService } from '../../services/wordService';

interface WordState {
  words: WordListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  search: string;
  isLoading: boolean;
  error: string | null;
  fetchWords: (params?: { page?: number; pageSize?: number; search?: string }) => Promise<void>;
  createWord: (payload: WordCreatePayload) => Promise<void>;
  updateWord: (id: string, payload: WordUpdatePayload) => Promise<void>;
  deleteWord: (id: string) => Promise<void>;
  saveWordCategories: (wordId: string, selectedCategoryIds: string[], originalCategoryIds: string[]) => Promise<void>;
}

export const useWordStore = create<WordState>((set, get) => ({
  words: [], totalCount: 0, page: 1, pageSize: 10, search: '', isLoading: false, error: null,

  fetchWords: async (params) => {
    const page = params?.page ?? get().page;
    const pageSize = params?.pageSize ?? get().pageSize;
    const search = params?.search ?? get().search;

    set({ isLoading: true, error: null });
    try {
      const result = await wordService.list(page, pageSize, search || undefined);
      set({ words: result.items, totalCount: result.totalCount, page: result.page, pageSize: result.pageSize, search, isLoading: false });
    } catch {
      set({ error: 'Erro ao carregar palavras.', isLoading: false });
    }
  },

  createWord: async (payload) => {
    await wordService.create(payload);
    await get().fetchWords();
  },

  updateWord: async (id, payload) => {
    await wordService.update(id, payload);
    await get().fetchWords();
  },

  deleteWord: async (id) => {
    await wordService.remove(id);
    set((state) => ({
      words: state.words.filter((w) => w.id !== id),
      totalCount: Math.max(0, state.totalCount - 1),
    }));
  },

  saveWordCategories: async (wordId, selectedCategoryIds, originalCategoryIds) => {
    const toAssociate = selectedCategoryIds.filter((id) => !originalCategoryIds.includes(id));
    const toDisassociate = originalCategoryIds.filter((id) => !selectedCategoryIds.includes(id));

    await Promise.all([
      ...toAssociate.map((categoryId) => wordService.associateCategory(wordId, categoryId)),
      ...toDisassociate.map((categoryId) => wordService.disassociateCategory(wordId, categoryId)),
    ]);
  },
}));
