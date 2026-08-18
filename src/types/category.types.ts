export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
}

export interface CategoryCreatePayload {
  name: string;
  description?: string;
}

export type CategoryUpdatePayload = CategoryCreatePayload;
