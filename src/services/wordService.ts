import { api } from './api';
import type { PaginatedResult } from '../types/common.types';
import type { BulkUploadResult, WordCreatePayload, WordDetail, WordListItem, WordUpdatePayload } from '../types/word.types';

export const wordService = {
  list: async (page: number, pageSize: number, search?: string): Promise<PaginatedResult<WordListItem>> => {
    const response = await api.get<PaginatedResult<WordListItem>>('/admin/words', { params: { page, pageSize, search } });
    return response.data;
  },
  getById: async (id: string): Promise<WordDetail> => {
    const response = await api.get<WordDetail>(`/admin/words/${id}`);
    return response.data;
  },
  create: async (payload: WordCreatePayload): Promise<WordDetail> => {
    const response = await api.post<WordDetail>('/admin/words', payload);
    return response.data;
  },
  update: async (id: string, payload: WordUpdatePayload): Promise<WordDetail> => {
    const response = await api.put<WordDetail>(`/admin/words/${id}`, payload);
    return response.data;
  },
  remove: async (id: string): Promise<void> => {
    await api.delete(`/admin/words/${id}`);
  },
  associateCategory: async (wordId: string, categoryId: string): Promise<void> => {
    await api.post(`/admin/words/${wordId}/categories/${categoryId}`);
  },
  disassociateCategory: async (wordId: string, categoryId: string): Promise<void> => {
    await api.delete(`/admin/words/${wordId}/categories/${categoryId}`);
  },
  bulkUploadCsv: async (file: File): Promise<BulkUploadResult> => {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post<BulkUploadResult>('/admin/words/bulk-upload', formData, {
      headers: { 'Content-Type': undefined },
    });
    return response.data;
  },
};
