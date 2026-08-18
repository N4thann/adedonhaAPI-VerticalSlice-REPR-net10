import { create } from 'zustand';
import type { AboutSite, AboutSiteUpsertPayload } from '../../types/aboutSite.types';
import { aboutSiteService } from '../../services/aboutSiteService';

interface AboutSiteState {
  data: AboutSite | null;
  isLoading: boolean;
  error: string | null;
  fetchAboutSite: () => Promise<void>;
  saveAboutSite: (payload: AboutSiteUpsertPayload) => Promise<void>;
}

export const useAboutSiteStore = create<AboutSiteState>((set) => ({
  data: null, isLoading: false, error: null,

  fetchAboutSite: async () => {
    set({ isLoading: true, error: null });
    try {
      const data = await aboutSiteService.get();
      set({ data, isLoading: false });
    } catch {
      set({ error: 'Erro ao carregar o conteúdo Sobre o site.', isLoading: false });
    }
  },

  saveAboutSite: async (payload) => {
    const data = await aboutSiteService.upsert(payload);
    set({ data });
  },
}));
