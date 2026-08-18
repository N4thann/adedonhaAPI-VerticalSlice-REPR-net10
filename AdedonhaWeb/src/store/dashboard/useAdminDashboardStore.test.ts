import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useAdminDashboardStore } from './useAdminDashboardStore';
import { adminDashboardService } from '../../services/adminDashboardService';

vi.mock('../../services/adminDashboardService', () => ({
  adminDashboardService: { getCategoryWordCounts: vi.fn() },
}));

describe('useAdminDashboardStore', () => {
  beforeEach(() => {
    useAdminDashboardStore.setState({ categoryWordCounts: [], isLoading: false, error: null });
    vi.mocked(adminDashboardService.getCategoryWordCounts).mockReset();
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
});
