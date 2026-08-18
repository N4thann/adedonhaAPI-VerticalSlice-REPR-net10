import { create } from 'zustand';
import type { Category, CategoryCreatePayload, CategoryUpdatePayload } from '../../types/category.types';
import { categoryService } from '../../services/categoryService';

interface CategoryState {
  categories: Category[];
  totalCount: number;
  page: number;
  pageSize: number;
  search: string;
  isLoading: boolean;
  error: string | null;
  fetchCategories: (params?: { page?: number; pageSize?: number; search?: string }) => Promise<void>;
  createCategory: (payload: CategoryCreatePayload) => Promise<void>;
  updateCategory: (id: string, payload: CategoryUpdatePayload) => Promise<void>;
  deleteCategory: (id: string) => Promise<void>;
}

export const useCategoryStore = create<CategoryState>((set, get) => ({
  categories: [], totalCount: 0, page: 1, pageSize: 10, search: '', isLoading: false, error: null,

  fetchCategories: async (params) => {
    const page = params?.page ?? get().page;
    const pageSize = params?.pageSize ?? get().pageSize;
    const search = params?.search ?? get().search;

    set({ isLoading: true, error: null });
    try {
      const result = await categoryService.list(page, pageSize, search || undefined);
      set({ categories: result.items, totalCount: result.totalCount, page: result.page, pageSize: result.pageSize, search, isLoading: false });
    } catch {
      set({ error: 'Erro ao carregar categorias.', isLoading: false });
    }
  },

  createCategory: async (payload) => {
    await categoryService.create(payload);
    await get().fetchCategories();
  },

  updateCategory: async (id, payload) => {
    await categoryService.update(id, payload);
    await get().fetchCategories();
  },

  deleteCategory: async (id) => {
    await categoryService.remove(id);
    set((state) => ({
      categories: state.categories.filter((c) => c.id !== id),
      totalCount: Math.max(0, state.totalCount - 1),
    }));
  },
}));
