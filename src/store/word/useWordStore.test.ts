import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useWordStore } from './useWordStore';
import { wordService } from '../../services/wordService';

vi.mock('../../services/wordService', () => ({
  wordService: {
    list: vi.fn(), create: vi.fn(), update: vi.fn(), remove: vi.fn(),
    associateCategory: vi.fn(), disassociateCategory: vi.fn(), bulkUploadCsv: vi.fn(),
  },
}));

const sampleResult = {
  items: [{ id: '1', name: 'Abacaxi', slug: 'abacaxi', initialLetter: 'A', description: undefined, categoryNames: ['Frutas'] }],
  totalCount: 1, page: 1, pageSize: 10,
};

describe('useWordStore', () => {
  beforeEach(() => {
    useWordStore.setState({ words: [], totalCount: 0, page: 1, pageSize: 10, search: '', isLoading: false, error: null });
    vi.mocked(wordService.list).mockReset();
    vi.mocked(wordService.remove).mockReset();
    vi.mocked(wordService.associateCategory).mockReset();
    vi.mocked(wordService.disassociateCategory).mockReset();
  });

  it('popula a lista quando fetchWords tem sucesso', async () => {
    vi.mocked(wordService.list).mockResolvedValue(sampleResult);
    await useWordStore.getState().fetchWords();
    expect(useWordStore.getState().words).toEqual(sampleResult.items);
  });

  it('remove a palavra da lista local ao excluir', async () => {
    useWordStore.setState({ words: sampleResult.items, totalCount: 1 });
    vi.mocked(wordService.remove).mockResolvedValue(undefined);
    await useWordStore.getState().deleteWord('1');
    expect(useWordStore.getState().words).toHaveLength(0);
  });

  it('associa apenas categorias novas e desassocia apenas as removidas', async () => {
    vi.mocked(wordService.associateCategory).mockResolvedValue(undefined);
    vi.mocked(wordService.disassociateCategory).mockResolvedValue(undefined);

    await useWordStore.getState().saveWordCategories('word-1', ['cat-a', 'cat-c'], ['cat-a', 'cat-b']);

    expect(wordService.associateCategory).toHaveBeenCalledTimes(1);
    expect(wordService.associateCategory).toHaveBeenCalledWith('word-1', 'cat-c');
    expect(wordService.disassociateCategory).toHaveBeenCalledTimes(1);
    expect(wordService.disassociateCategory).toHaveBeenCalledWith('word-1', 'cat-b');
  });

  it('não chama associate/disassociate quando as categorias não mudaram', async () => {
    await useWordStore.getState().saveWordCategories('word-1', ['cat-a'], ['cat-a']);
    expect(wordService.associateCategory).not.toHaveBeenCalled();
    expect(wordService.disassociateCategory).not.toHaveBeenCalled();
  });
});
