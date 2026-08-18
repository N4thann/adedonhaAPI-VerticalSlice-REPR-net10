import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useCategoryStore } from './useCategoryStore';
import { categoryService } from '../../services/categoryService';

vi.mock('../../services/categoryService', () => ({
  categoryService: { list: vi.fn(), create: vi.fn(), update: vi.fn(), remove: vi.fn() },
}));

const sampleResult = {
  items: [{ id: '1', name: 'Animais', slug: 'animais', description: 'desc' }],
  totalCount: 1, page: 1, pageSize: 10,
};

describe('useCategoryStore', () => {
  beforeEach(() => {
    useCategoryStore.setState({ categories: [], totalCount: 0, page: 1, pageSize: 10, search: '', isLoading: false, error: null });
    vi.mocked(categoryService.list).mockReset();
    vi.mocked(categoryService.remove).mockReset();
  });

  it('popula a lista quando fetchCategories tem sucesso', async () => {
    vi.mocked(categoryService.list).mockResolvedValue(sampleResult);

    await useCategoryStore.getState().fetchCategories();

    const state = useCategoryStore.getState();
    expect(state.categories).toEqual(sampleResult.items);
    expect(state.totalCount).toBe(1);
    expect(state.isLoading).toBe(false);
    expect(state.error).toBeNull();
  });

  it('marca error quando fetchCategories falha', async () => {
    vi.mocked(categoryService.list).mockRejectedValue(new Error('falhou'));

    await useCategoryStore.getState().fetchCategories();

    const state = useCategoryStore.getState();
    expect(state.error).not.toBeNull();
    expect(state.isLoading).toBe(false);
  });

  it('remove a categoria da lista local ao excluir', async () => {
    useCategoryStore.setState({ categories: sampleResult.items, totalCount: 1 });
    vi.mocked(categoryService.remove).mockResolvedValue(undefined);

    await useCategoryStore.getState().deleteCategory('1');

    const state = useCategoryStore.getState();
    expect(state.categories).toHaveLength(0);
    expect(state.totalCount).toBe(0);
  });
});
