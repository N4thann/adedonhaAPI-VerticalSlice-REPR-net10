import { api } from './api';
import type { AboutSite, AboutSiteUpsertPayload } from '../types/aboutSite.types';

export const aboutSiteService = {
  get: async (): Promise<AboutSite> => {
    const response = await api.get<AboutSite>('/catalog/about-site');
    return response.data;
  },
  upsert: async (payload: AboutSiteUpsertPayload): Promise<AboutSite> => {
    const formData = new FormData();
    formData.append('Cargo', payload.cargo);
    payload.formacoes.forEach((formacao) => formData.append('Formacoes', formacao));
    formData.append('TextoGeral', payload.textoGeral);
    payload.tecnologias.forEach((tecnologia) => formData.append('Tecnologias', tecnologia));
    payload.arquiteturas.forEach((arquitetura) => formData.append('Arquiteturas', arquitetura));
    if (payload.image) formData.append('Image', payload.image);

    const response = await api.put<AboutSite>('/admin/about-site', formData, {
      headers: { 'Content-Type': undefined },
    });
    return response.data;
  },
};
