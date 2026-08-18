import { create } from 'zustand';
import type { CategoryWordCount } from '../../types/categoryWordCount.types';
import type { WordStats } from '../../types/wordStats.types';
import { adminDashboardService } from '../../services/adminDashboardService';

interface AdminDashboardState {
  categoryWordCounts: CategoryWordCount[];
  isLoading: boolean;
  error: string | null;
  wordStats: WordStats | null;
  isLoadingWordStats: boolean;
  wordStatsError: string | null;
  fetchCategoryWordCounts: () => Promise<void>;
  fetchWordStats: () => Promise<void>;
}

export const useAdminDashboardStore = create<AdminDashboardState>((set) => ({
  categoryWordCounts: [], isLoading: false, error: null,
  wordStats: null, isLoadingWordStats: false, wordStatsError: null,

  fetchCategoryWordCounts: async () => {
    set({ isLoading: true, error: null });
    try {
      const items = await adminDashboardService.getCategoryWordCounts();
      set({ categoryWordCounts: items, isLoading: false });
    } catch {
      set({ error: 'Erro ao carregar dados do dashboard.', isLoading: false });
    }
  },

  fetchWordStats: async () => {
    set({ isLoadingWordStats: true, wordStatsError: null });
    try {
      const wordStats = await adminDashboardService.getWordStats();
      set({ wordStats, isLoadingWordStats: false });
    } catch {
      set({ wordStatsError: 'Erro ao carregar estatísticas de palavras.', isLoadingWordStats: false });
    }
  },
}));
