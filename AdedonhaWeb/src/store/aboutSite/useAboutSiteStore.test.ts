import { describe, it, expect, vi, beforeEach } from 'vitest';
import { useAboutSiteStore } from './useAboutSiteStore';
import { aboutSiteService } from '../../services/aboutSiteService';

vi.mock('../../services/aboutSiteService', () => ({
  aboutSiteService: { get: vi.fn(), upsert: vi.fn() },
}));

const sampleData = {
  cargo: 'Engenheiro de Software',
  formacoes: ['Ciência da Computação'],
  textoGeral: 'Texto de exemplo.',
  tecnologias: ['.NET', 'React'],
  arquiteturas: ['Clean Architecture'],
  imageUrl: null,
};

const samplePayload = { cargo: 'x', formacoes: [], textoGeral: 'y', tecnologias: [], arquiteturas: [] };

describe('useAboutSiteStore', () => {
  beforeEach(() => {
    useAboutSiteStore.setState({ data: null, isLoading: false, error: null });
    vi.mocked(aboutSiteService.get).mockReset();
    vi.mocked(aboutSiteService.upsert).mockReset();
  });

  it('popula data quando fetchAboutSite tem sucesso', async () => {
    vi.mocked(aboutSiteService.get).mockResolvedValue(sampleData);

    await useAboutSiteStore.getState().fetchAboutSite();

    const state = useAboutSiteStore.getState();
    expect(state.data).toEqual(sampleData);
    expect(state.isLoading).toBe(false);
    expect(state.error).toBeNull();
  });

  it('marca error quando fetchAboutSite falha', async () => {
    vi.mocked(aboutSiteService.get).mockRejectedValue(new Error('falhou'));

    await useAboutSiteStore.getState().fetchAboutSite();

    const state = useAboutSiteStore.getState();
    expect(state.error).not.toBeNull();
    expect(state.isLoading).toBe(false);
  });

  it('atualiza data quando saveAboutSite tem sucesso', async () => {
    vi.mocked(aboutSiteService.upsert).mockResolvedValue(sampleData);

    await useAboutSiteStore.getState().saveAboutSite(samplePayload);

    expect(useAboutSiteStore.getState().data).toEqual(sampleData);
  });

  it('propaga o erro quando saveAboutSite falha, sem alterar data', async () => {
    vi.mocked(aboutSiteService.upsert).mockRejectedValue(new Error('falhou'));

    await expect(useAboutSiteStore.getState().saveAboutSite(samplePayload)).rejects.toThrow();

    expect(useAboutSiteStore.getState().data).toBeNull();
  });
});
