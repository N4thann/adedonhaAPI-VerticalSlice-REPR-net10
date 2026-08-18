import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useCategoryWordsStore } from './useCategoryWordsStore';
import { catalogWordService } from '../../services/catalogWordService';

vi.mock('../../services/catalogWordService', () => ({
  catalogWordService: { listByCategoryAndLetter: vi.fn() },
}));

function buildBatch(count: number, offset = 0) {
  return Array.from({ length: count }, (_, i) => ({ name: `Palavra ${i + offset}`, slug: `palavra-${i + offset}` }));
}

describe('useCategoryWordsStore', () => {
  beforeEach(() => {
    useCategoryWordsStore.setState({ words: [], page: 0, pageSize: 10, seed: 0, hasMore: true, isLoading: false, error: null });
    vi.mocked(catalogWordService.listByCategoryAndLetter).mockReset();
  });

  it('popula a lista na busca inicial', async () => {
    vi.mocked(catalogWordService.listByCategoryAndLetter).mockResolvedValue({ items: buildBatch(10), totalCount: 25, page: 1, pageSize: 10 });

    await useCategoryWordsStore.getState().initialize('objetos', 'A', 10);

    const state = useCategoryWordsStore.getState();
    expect(state.words).toHaveLength(10);
    expect(state.page).toBe(1);
    expect(state.hasMore).toBe(true);
  });

  it('concatena o proximo lote ao chamar loadNextPage', async () => {
    vi.mocked(catalogWordService.listByCategoryAndLetter)
      .mockResolvedValueOnce({ items: buildBatch(10), totalCount: 25, page: 1, pageSize: 10 })
      .mockResolvedValueOnce({ items: buildBatch(10, 10), totalCount: 25, page: 2, pageSize: 10 });

    await useCategoryWordsStore.getState().initialize('objetos', 'A', 10);
    await useCategoryWordsStore.getState().loadNextPage('objetos', 'A');

    const state = useCategoryWordsStore.getState();
    expect(state.words).toHaveLength(20);
    expect(state.page).toBe(2);
  });

  it('marca hasMore como false quando o lote vem menor que o pageSize', async () => {
    vi.mocked(catalogWordService.listByCategoryAndLetter).mockResolvedValue({ items: buildBatch(3), totalCount: 3, page: 1, pageSize: 10 });

    await useCategoryWordsStore.getState().initialize('objetos', 'A', 10);

    expect(useCategoryWordsStore.getState().hasMore).toBe(false);
  });

  it('marca error quando a busca inicial falha', async () => {
    vi.mocked(catalogWordService.listByCategoryAndLetter).mockRejectedValue(new Error('falhou'));

    await useCategoryWordsStore.getState().initialize('objetos', 'A', 10);

    expect(useCategoryWordsStore.getState().error).not.toBeNull();
    expect(useCategoryWordsStore.getState().isLoading).toBe(false);
  });
});
