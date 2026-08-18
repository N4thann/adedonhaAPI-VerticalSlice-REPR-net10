import { create } from 'zustand';
import type { CategoryWordCount } from '../../types/categoryWordCount.types';
import { adminDashboardService } from '../../services/adminDashboardService';

interface AdminDashboardState {
  categoryWordCounts: CategoryWordCount[];
  isLoading: boolean;
  error: string | null;
  fetchCategoryWordCounts: () => Promise<void>;
}

export const useAdminDashboardStore = create<AdminDashboardState>((set) => ({
  categoryWordCounts: [], isLoading: false, error: null,

  fetchCategoryWordCounts: async () => {
    set({ isLoading: true, error: null });
    try {
      const items = await adminDashboardService.getCategoryWordCounts();
      set({ categoryWordCounts: items, isLoading: false });
    } catch {
      set({ error: 'Erro ao carregar dados do dashboard.', isLoading: false });
    }
  },
}));
