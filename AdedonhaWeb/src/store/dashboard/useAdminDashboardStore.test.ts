import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useAdminDashboardStore } from './useAdminDashboardStore';
import { adminDashboardService } from '../../services/adminDashboardService';

vi.mock('../../services/adminDashboardService', () => ({
  adminDashboardService: { getCategoryWordCounts: vi.fn(), getWordStats: vi.fn() },
}));

describe('useAdminDashboardStore', () => {
  beforeEach(() => {
    useAdminDashboardStore.setState({
      categoryWordCounts: [], isLoading: false, error: null,
      wordStats: null, isLoadingWordStats: false, wordStatsError: null,
    });
    vi.mocked(adminDashboardService.getCategoryWordCounts).mockReset();
    vi.mocked(adminDashboardService.getWordStats).mockReset();
  });

  it('popula a lista quando fetchCategoryWordCounts tem sucesso', async () => {
    const items = [{ name: 'Animais', slug: 'animais', wordCount: 12 }];
    vi.mocked(adminDashboardService.getCategoryWordCounts).mockResolvedValue(items);

    await useAdminDashboardStore.getState().fetchCategoryWordCounts();

    const state = useAdminDashboardStore.getState();
    expect(state.categoryWordCounts).toEqual(items);
    expect(state.isLoading).toBe(false);
    expect(state.error).toBeNull();
  });

  it('marca error quando fetchCategoryWordCounts falha', async () => {
    vi.mocked(adminDashboardService.getCategoryWordCounts).mockRejectedValue(new Error('falhou'));

    await useAdminDashboardStore.getState().fetchCategoryWordCounts();

    const state = useAdminDashboardStore.getState();
    expect(state.error).not.toBeNull();
    expect(state.isLoading).toBe(false);
  });

  it('popula wordStats quando fetchWordStats tem sucesso', async () => {
    const stats = { totalWords: 5 };
    vi.mocked(adminDashboardService.getWordStats).mockResolvedValue(stats);

    await useAdminDashboardStore.getState().fetchWordStats();

    const state = useAdminDashboardStore.getState();
    expect(state.wordStats).toEqual(stats);
    expect(state.isLoadingWordStats).toBe(false);
    expect(state.wordStatsError).toBeNull();
  });

  it('marca wordStatsError quando fetchWordStats falha', async () => {
    vi.mocked(adminDashboardService.getWordStats).mockRejectedValue(new Error('falhou'));

    await useAdminDashboardStore.getState().fetchWordStats();

    const state = useAdminDashboardStore.getState();
    expect(state.wordStatsError).not.toBeNull();
    expect(state.isLoadingWordStats).toBe(false);
  });
});
